namespace xMap
{
    using System.Collections.Generic;
    using System.Linq;

    using global::xMap.Exceptions;
    using global::xMap.Generators;

    public class xMapOptions<TSource, TFor>
        where TSource : class
    {
        public TFor ConditionTarget { get; set; }

        public TSource Source { get; set; }

        public TDest To<TDest>()
        {
            var generator = new QueryGenerator<TSource>();
            var result = generator.GenerateFor<TSource, TDest, TFor>(new List<TSource> { Source }.AsQueryable(), ConditionTarget);

            if (result == null)
            {
                throw new NoMappingsFoundException();
            }

            return result.FirstOrDefault();
        }
    }

    public class xMapQueryOptions<TSource, TFor>
        where TSource : class
    {
        public TFor ConditionTarget { get; set; }

        public IQueryable<TSource> Source { get; set; }

        public IQueryable<TDest> To<TDest>()
        {
            return xMap.MapFor<TSource, TDest, TFor>(Source, typeof(TSource), ConditionTarget);
        }
    }
}
