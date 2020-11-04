using System;

namespace CM.Backend.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public static DomainException CausedBy(string cause, Exception innerException = null)
        {
            return new DomainException(cause, innerException);
        }

        public DomainException(string message, Exception innerException = null) : base(message, innerException) 
        {}
    }
}