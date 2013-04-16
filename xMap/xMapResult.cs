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

            var thisMap = this.map;
            var thisExpression = (Expression<Func<TSource, TDestination>>)thisMap.Expression;

            var modifiedBaseExpression = ExpressionHelper.Retrofit(baseExpression, thisExpression);
            xMap.Mappings[typeof(TBase)].Remove(baseMap);
            xMap.Mappings[typeof(TBase)].Add(new xMapping { TargetType = typeof(TDestination), Expression = modifiedBaseExpression });

            // combine this expression with the one just added and map to the list for TSource
            var combinedExpression = ExpressionHelper.Merge(baseExpression, thisExpression);

            // remove the old map and add the new one
            xMap.Mappings[typeof(TSource)].Remove(thisMap);
            xMap.Mappings[typeof(TSource)].Add(new xMapping { TargetType = typeof(TDestination), Expression = combinedExpression });

            return this;
        }

        public void WithPartial(xMapPartial setName)
        {
            throw new NotImplementedException();
        }
    }
}