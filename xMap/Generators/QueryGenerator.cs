namespace xMap.Generators
{
    using System;
    using System.Linq;

    using global::xMap.Exceptions;

    internal abstract class QueryGenerator
    {
        public abstract IQueryable<TOut> Generate<T, TOut>(IQueryable<T> input);

        public abstract IQueryable<TOut> GenerateFor<T, TOut, TFor>(IQueryable<T> input, TFor conditionTarget);

        public abstract Type Type();
    }

    internal class QueryGenerator<TDerived> : QueryGenerator where TDerived : class
    {
        public override IQueryable<TOut> Generate<T, TOut>(IQueryable<T> input)
        {
            return input.OfType<TDerived>().Select(xMap.GetMapExpression<TDerived, TOut>());
        }

        public override IQueryable<TOut> GenerateFor<T, TOut, TFor>(IQueryable<T> input, TFor conditionTarget)
        {
            var expression = xMap.GetMapExpression<TDerived, TOut, TFor>(conditionTarget);
            if (expression == null)
            {
                return null;
            }

            return input.OfType<TDerived>().Select(expression);
        }

        public override Type Type()
        {
            return typeof(TDerived);
        }
    }
}