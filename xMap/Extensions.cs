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
    }
}