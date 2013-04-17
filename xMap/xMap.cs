namespace xMap
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    using global::xMap.Expressions;
    using global::xMap.Generators;

    public static class xMap
    {
        internal static ConcurrentDictionary<Type, List<xMapping>> Mappings { get; set; }

        internal static ConcurrentDictionary<Type, List<QueryGenerator>> Relationships { get; set; }

        static xMap()
        {
            Mappings = new ConcurrentDictionary<Type, List<xMapping>>();
            Relationships = new ConcurrentDictionary<Type, List<QueryGenerator>>();
        }

        public static void Reset()
        {
            Mappings.Clear();
            Relationships.Clear();
        }

        public static xMapResult<TSource, TDestination> Define<TSource, TDestination>(Expression<Func<TSource, TDestination>> expression) where TSource : class
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            if (!Mappings.ContainsKey(sourceType))
            {
                Mappings.TryAdd(sourceType, new List<xMapping>());
            }

            var map = new xMapping { TargetType = destinationType, Expression = expression };
            Mappings[typeof(TSource)].Add(map);

            return new xMapResult<TSource, TDestination>(map);
        }

        public static IQueryable<TOut> Map<T, TOut>(IQueryable<T> input, Type baseType) where T : class
        {
            var typeMaps = new List<QueryGenerator>();

            // Add types that derive from baseType
            if (Relationships.ContainsKey(baseType))
            {
                typeMaps.AddRange(Relationships[baseType]);
            }

            if (typeMaps.Count == 0)
            {
                // Include a map for the type itself in the case that we aren't doing anything clever with inheritance
                typeMaps.Add(new QueryGenerator<T>());

                var typemap = typeMaps.First();
                return typemap.Generate<T, TOut>(input);
            }

            var queries = typeMaps.Select(generator => generator.Generate<T, TOut>(input));

            IQueryable<TOut> result = null;
            foreach (IQueryable<TOut> query in queries)
            {
                result = result == null ? query : result.Concat(query);
            }

            return result;
        }

        internal static Expression<Func<TDerived, TOut>> GetMapExpression<TDerived, TOut>()
        {
            Type type = typeof(TDerived);
            Type destinationType = typeof(TOut);

            var expression = Mappings[type].Single(o => o.TargetType == destinationType).Expression;
            var relationship = Relationships.Where(o => o.Value.Any(q => q.Type() == type)).Select(o => o.Key).FirstOrDefault();

            if (relationship != null)
            {
                var baseExpression = Mappings[relationship].Single(o => o.TargetType == destinationType);
                // combine this expression with the base
                var combinedExpression = ExpressionHelper.Merge<TDerived, TOut>(baseExpression.Expression, expression);
                return combinedExpression;
            }

            return (Expression<Func<TDerived, TOut>>)expression;
        }
    }
}