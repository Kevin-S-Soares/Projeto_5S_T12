using System;

namespace WebOdontologista.Models.Exceptions
{
    public class DomainException : ApplicationException
    {
        public DomainException(string message) : base(message) { }
    }
}
