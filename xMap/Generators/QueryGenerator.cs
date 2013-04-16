namespace xMap.Generators
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal abstract class QueryGenerator
    {
        public abstract IQueryable<TOut> Generate<T, TOut>(IQueryable<T> input);

        public abstract Type Type();
    }

    internal class QueryGenerator<TDerived> : QueryGenerator where TDerived : class
    {
        public override IQueryable<TOut> Generate<T, TOut>(IQueryable<T> input)
        {
            return
                input.OfType<TDerived>().Select(
                    (Expression<Func<TDerived, TOut>>)xMap.Map(typeof(TDerived), typeof(TOut)));
        }

        public override Type Type()
        {
            return typeof(TDerived);
        }
    }
}