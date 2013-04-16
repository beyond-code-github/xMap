namespace xMap.Generators
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal abstract class KeyExpressionGenerator
    {
        public abstract IQueryable<TOut> Generate<TOut>(IQueryable<TOut> input);
    }

    internal class KeyExpressionGenerator<TKey> : KeyExpressionGenerator where TKey : struct
    {
        private readonly Expression keyExpression;

        public KeyExpressionGenerator(Expression keyExpression)
        {
            this.keyExpression = keyExpression;
        }

        public override IQueryable<TOut> Generate<TOut>(IQueryable<TOut> input)
        {
            return input.GroupBy((Expression<Func<TOut, TKey>>)keyExpression).Select(o => o.FirstOrDefault());
        }
    }
}
