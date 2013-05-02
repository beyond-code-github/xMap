namespace xMap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using global::xMap.Exceptions;
    using global::xMap.Expressions;
    using global::xMap.Generators;

    public class xMapResult<TSource, TDestination> where TSource : class
    {
        protected readonly xMapping map;

        public xMapResult(xMapping map)
        {
            this.map = map;
        }

        public xMapResult<TSource, TDestination> DerivedFrom<TBase>()
        {
            // Add a relationship between TBase and TSource
            if (!xMap.Relationships.ContainsKey(typeof(TBase)))
            {
                xMap.Relationships.TryAdd(typeof(TBase), new List<QueryGenerator>());
            }

            xMap.Relationships[typeof(TBase)].Add(new QueryGenerator<TSource>());

            // find expression map targeting TBase
            var baseMap = xMap.Mappings[typeof(TBase)].Single(o => o.TargetType == typeof(TDestination));
            var baseExpression = (Expression<Func<TBase, TDestination>>)baseMap.Expression;
            var thisExpression = (Expression<Func<TSource, TDestination>>)this.map.Expression;

            // Copy condition if applicable - this will override any conditions on the derived map
            if (baseMap.Condition != null)
            {
                this.map.Condition = baseMap.Condition;
            }

            // Retrofit the base expression with any additional mappings that aren't present
            var modifiedBaseExpression = ExpressionHelper.Retrofit(baseExpression, thisExpression);

            xMap.Mappings[typeof(TBase)].Remove(baseMap);
            xMap.Mappings[typeof(TBase)].Add(
                new xMapping
                    {
                        Condition = baseMap.Condition,
                        UseDefaultsWhenConditional = baseMap.UseDefaultsWhenConditional,
                        TargetType = typeof(TDestination),
                        Expression = modifiedBaseExpression
                    });

            return this;
        }

        public xMapResult<TSource, TDestination> For<T>(Func<T, bool> predicate)
        {
            if (this.map.Condition != null)
            {
                throw new ConditionAlreadySpecifiedException();
            }

            this.map.Condition = new MapCondition<T>(predicate);
            return this;
        }

        public xMapResult<TSource, TDestination> WithDefaults()
        {
            this.map.UseDefaultsWhenConditional = true;
            return this;
        }
    }
}