namespace xMap
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class MemberBindingComparer : IComparer<MemberBinding>
    {
        public int Compare(MemberBinding x, MemberBinding y)
        {
            return System.String.Compare(x.Member.Name, y.Member.Name, System.StringComparison.Ordinal);
        }
    }
}
