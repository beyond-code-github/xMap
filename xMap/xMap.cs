namespace xMap
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    using global::xMap.Exceptions;
    using global::xMap.Generators;

    public static class xMap
    {
        internal static ConcurrentDictionary<Type, List<xMapping>> Mappings { get; set; }

        internal static ConcurrentDictionary<Type, List<QueryGenerator>> Relationships { get; set; }

        internal static ConcurrentDictionary<Type, KeyExpressionGenerator> Keys { get; set; }

        static xMap()
        {
            Mappings = new ConcurrentDictionary<Type, List<xMapping>>();
            Relationships = new ConcurrentDictionary<Type, List<QueryGenerator>>();
            Keys = new ConcurrentDictionary<Type, KeyExpressionGenerator>();
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

        /// <summary>
        /// Maps the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>LINQ Expression</returns>
        internal static Expression Map(Type type, Type destinationType)
        {
            var result = Mappings[type].Single(o => o.TargetType == destinationType).Expression;
            return result;
        }

        public static xMapPartial Partial(Expression<Func<DynamicObject, object>> func)
        {
            throw new NotImplementedException();
        }
    }
}