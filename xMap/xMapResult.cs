namespace xMap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using global::xMap.Expressions;
    using global::xMap.Generators;

    public class xMapResult<TSource, TDestination> where TSource : class
    {
        private readonly xMapping map;

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

            // Retrofit the base expression with any additional mappings that aren't present
            var modifiedBaseExpression = ExpressionHelper.Retrofit(baseExpression, thisExpression);

            xMap.Mappings[typeof(TBase)].Remove(baseMap);
            xMap.Mappings[typeof(TBase)].Add(new xMapping { TargetType = typeof(TDestination), Expression = modifiedBaseExpression });

            return this;
        }
    }
}