namespace xMap.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Used to combine two initialization expressions
    /// </summary>
    internal class ExpressionComposer<TOut> : ExpressionVisitor
    {
        private readonly List<MemberBinding> bindings;

        private readonly Dictionary<string, ParameterExpression> parameters;

        public ExpressionComposer()
        {
            this.bindings = new List<MemberBinding>();
            this.parameters = new Dictionary<string, ParameterExpression>();
        }

        public Expression<Func<T, TOut>> BuildComposite<T>()
        {
            return Expression.Lambda<Func<T, TOut>>(
                Expression.MemberInit(Expression.New(typeof(TOut)), this.bindings), this.parameters.Values);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (!this.parameters.ContainsKey(node.Name))
            {
                this.parameters.Add(node.Name, node);
            }

            return base.VisitParameter(node);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            var existingBinding = this.bindings.FirstOrDefault(o => o.Member.Name == node.Member.Name);

            if (existingBinding != null)
            {
                this.bindings.Remove(existingBinding);
            }

            this.bindings.Add(node);
            return base.VisitMemberBinding(node);
        }
    }
}