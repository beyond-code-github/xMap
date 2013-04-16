namespace xMap
{
    using System.Linq;

    public static class Extensions
    {
        public static IQueryable<TDestination> Map<TBase, TDestination>(this IQueryable<TBase> input) where TBase : class
        {
            return xMap.Map<TBase, TDestination>(input, typeof(TBase));
        }
    }
}