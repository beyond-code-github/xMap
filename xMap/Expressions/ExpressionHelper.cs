namespace xMap.Expressions
{
    using System;
    using System.Linq.Expressions;

    internal class ExpressionHelper
    {
        /// <summary>
        /// Merges the specified expressions. Assignments from both expressions will be used but the parameters will be adjusted to use only those from expression2
        /// </summary>
        public static Expression<Func<TDerived, TOut>> Merge<TDerived, TOut>(LambdaExpression expression1, LambdaExpression expression2)
        {
            var composer = new ExpressionComposer<TOut>();

            var modified = new ParameterVisitor(expression1.Parameters, expression2.Parameters).Visit(expression1.Body);

            composer.Visit(modified);
            composer.Visit(expression2);

            return composer.BuildComposite<TDerived>();
        }

        // Updates baseExpression to include any member init bindings from thisExpression
        public static Expression<Func<TBase, TOut>> Retrofit<TBase, TDerived, TOut>(Expression<Func<TBase, TOut>> baseExpression, Expression<Func<TDerived, TOut>> thisExpression)
        {
            var memberInit = thisExpression.Body as MemberInitExpression;

            if (memberInit != null)
            {
                var modified = (LambdaExpression)new AssignmentVisitor(memberInit.Bindings).Visit(baseExpression);
                var modifiedBody = (MemberInitExpression)modified.Body;

                return Expression.Lambda<Func<TBase, TOut>>(
                Expression.MemberInit(Expression.New(typeof(TOut)), modifiedBody.Bindings), modified.Parameters);
            }

            return baseExpression;
        }
    }
}