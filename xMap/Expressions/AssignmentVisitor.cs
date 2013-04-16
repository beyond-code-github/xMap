namespace xMap.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Linq.Expressions;
    using System.Linq;

    public class AssignmentVisitor : ExpressionVisitor
    {
        private readonly ReadOnlyCollection<MemberBinding> fromBindings;

        public AssignmentVisitor(ReadOnlyCollection<MemberBinding> fromBindings)
        {
            this.fromBindings = fromBindings;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var bindings = node.Bindings.ToList();

            foreach (var binding in this.fromBindings)
            {
                if (bindings.All(o => o.Member.Name != binding.Member.Name) && binding.Member is PropertyInfo)
                {
                    var propertyInfo = binding.Member as PropertyInfo;

                    switch (propertyInfo.PropertyType.ToString())
                    {
                        case "System.String":
                            bindings.Add(Expression.Bind(binding.Member, Expression.Constant("")));
                            break;
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Int16":
                            bindings.Add(Expression.Bind(binding.Member, Expression.Constant(0)));
                            break;
                        case "System.DateTime":
                            bindings.Add(Expression.Bind(binding.Member, Expression.Constant(DateTime.MinValue)));
                            break;
                    }
                }
            }

            return Expression.MemberInit(node.NewExpression, bindings);
        }
    }
}
