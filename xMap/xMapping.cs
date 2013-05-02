namespace xMap
{
    using System;
    using System.Linq.Expressions;

    public class xMapping
    {
        public Type TargetType { get; set; }

        public LambdaExpression Expression { get; set; }

        public MapCondition Condition { get; set; }

        public bool IsConditional
        {
            get
            {
                return this.Condition != null;
            }
        }

        public bool UseDefaultsWhenConditional { get; set; }

        public bool MeetsCondition<T>(T conditionTarget)
        {
            if (this.Condition == null)
            {
                return false;
            }

            if (this.Condition.Type() == typeof(T))
            {
                return this.Condition.Test(conditionTarget);
            }

            return false;
        }
    }

    public abstract class MapCondition
    {
        public abstract bool Test(object obj);

        public abstract Type Type();
    }

    public class MapCondition<T> : MapCondition
    {
        private readonly Func<T, bool> condition;

        public MapCondition(Func<T, bool> condition)
        {
            this.condition = condition;
        }

        public override bool Test(object obj)
        {
            return this.condition((T)obj);
        }

        public override Type Type()
        {
            return typeof(T);
        }
    }
}