namespace xMap
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    using global::xMap.Exceptions;
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

        public static IQueryable<TOut> MapFor<T, TOut, TFor>(IQueryable<T> input, Type baseType, TFor conditionTarget) where T : class
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
                var query = typemap.GenerateFor<T, TOut, TFor>(input, conditionTarget);
                if (query == null)
                {
                    throw new NoMappingsFoundException();
                }

                return query;
            }

            var queries = typeMaps.Select(generator => generator.GenerateFor<T, TOut, TFor>(input, conditionTarget)).Where(o => o != null);
            if (!queries.Any())
            {
                throw new NoMappingsFoundException();
            }

            IQueryable<TOut> result = null;
            foreach (IQueryable<TOut> query in queries)
            {
                result = result == null ? query : result.Concat(query);
            }

            return result;
        }

        internal static Expression<Func<TDerived, TOut>> GetMapExpression<TDerived, TOut>()
        {
            var type = typeof(TDerived);
            var destinationType = typeof(TOut);

            var expression = Mappings[type].Single(o => o.TargetType == destinationType).Expression;
            var relationship = Relationships.Where(o => o.Value.Any(q => q.Type() == type)).Select(o => o.Key).FirstOrDefault();

            if (relationship != null)
            {
                var baseExpression = Mappings[relationship].Single(o => o.TargetType == destinationType);
                return ExpressionHelper.Merge<TDerived, TOut>(baseExpression.Expression, expression);
            }

            return (Expression<Func<TDerived, TOut>>)expression;
        }

        internal static Expression<Func<TDerived, TOut>> GetMapExpression<TDerived, TOut, TFor>(TFor conditionTarget)
        {
            var type = typeof(TDerived);
            var destinationType = typeof(TOut);

            var relationship = Relationships.Where(o => o.Value.Any(q => q.Type() == type)).Select(o => o.Key).FirstOrDefault();

            // First try and see if there is a condition specific mapping
            var mapping =
                Mappings[type].SingleOrDefault(
                    o => o.TargetType == destinationType && o.MeetsCondition(conditionTarget));

            // Otherwise try and see if there is a default mapping
            var unconditionalMapping = Mappings[type].SingleOrDefault(
                    o => o.TargetType == destinationType && !o.IsConditional);

            if (mapping != null && mapping.UseDefaultsWhenConditional && unconditionalMapping != null)
            {
                var combinedExpression = ExpressionHelper.Merge<TDerived, TOut>(unconditionalMapping.Expression, mapping.Expression);
                if (relationship != null)
                {
                    var baseExpression = Mappings[relationship].Single(o => o.TargetType == destinationType);
                    combinedExpression = ExpressionHelper.Merge<TDerived, TOut>(baseExpression.Expression, combinedExpression);
                }

                return combinedExpression;
            }

            if (mapping == null && unconditionalMapping != null)
            {
                if (relationship != null)
                {
                    var baseExpression = Mappings[relationship].Single(o => o.TargetType == destinationType);
                    if (!baseExpression.IsConditional || baseExpression.MeetsCondition(conditionTarget))
                    {
                        return ExpressionHelper.Merge<TDerived, TOut>(baseExpression.Expression, unconditionalMapping.Expression);
                    }
                }

                return (Expression<Func<TDerived, TOut>>)unconditionalMapping.Expression;
            }

            if (mapping == null)
            {
                return null;
            }

            if (relationship != null)
            {
                var baseExpression = Mappings[relationship].Single(o => o.TargetType == destinationType);
                return ExpressionHelper.Merge<TDerived, TOut>(baseExpression.Expression, mapping.Expression);
            }

            return (Expression<Func<TDerived, TOut>>)mapping.Expression;
        }
    }
}