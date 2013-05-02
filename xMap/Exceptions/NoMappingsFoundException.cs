namespace xMap.Exceptions
{
    using System;

    public class NoMappingsFoundException : Exception
    {
        public NoMappingsFoundException()
            : base("No mappings were found for the specified type and\\or condition")
        {
        }
    }
}
