namespace xMap
{
    using System.Collections.Generic;

    public abstract class ResultComparerBase
    {
    }

    public abstract class ResultComparer<T> : ResultComparerBase, IEqualityComparer<T>
    {
        public abstract bool Equals(T x, T y);

        public abstract int GetHashCode(T obj);
    }

    internal class DefaultResultComparerAdapter<T> : ResultComparerBase, IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
