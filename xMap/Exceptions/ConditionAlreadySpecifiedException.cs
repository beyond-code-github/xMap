namespace xMap.Exceptions
{
    using System;

    public class ConditionAlreadySpecifiedException : Exception
    {
        public ConditionAlreadySpecifiedException()
            : base("A condition has already been specified for this map, or has been inherited from the base type of a derived map")
        {
        }
    }
}
