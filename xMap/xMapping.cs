namespace xMap
{
    using System;
    using System.Linq.Expressions;

    public class xMapping
    {
        public Type TargetType { get; set; }

        public Expression Expression { get; set; }
    }
}