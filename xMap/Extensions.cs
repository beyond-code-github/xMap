namespace xMap
{
    using System.Collections.Generic;
    using System.Linq;

    using global::xMap.Generators;

    public static class Extensions
    {
        public static IQueryable<TDestination> Map<TBase, TDestination>(this IQueryable<TBase> input) where TBase : class
        {
            return xMap.Map<TBase, TDestination>(input, typeof(TBase));
        }

        public static TOut Map<T, TOut>(this T input) where T : class
        {
            var generator = new QueryGenerator<T>();
            return generator.Generate<T, TOut>(new List<T> { input }.AsQueryable()).FirstOrDefault();
        }

        public static xMapOptions<TSource, TConditionTarget> MapFor<TSource, TConditionTarget>(this TSource input, TConditionTarget conditionTarget) where TSource : class
        {
            return new xMapOptions<TSource, TConditionTarget> { Source = input, ConditionTarget = conditionTarget };
        }

        public static xMapQueryOptions<TSource, TConditionTarget> MapFor<TSource, TConditionTarget>(this IQueryable<TSource> input, TConditionTarget conditionTarget) where TSource : class
        {
            return new xMapQueryOptions<TSource, TConditionTarget> { Source = input, ConditionTarget = conditionTarget };
        }
    }
}