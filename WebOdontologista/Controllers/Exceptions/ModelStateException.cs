using System;

namespace WebOdontologista.Controllers.Exceptions
{
    public class ModelStateException : ApplicationException
    {
        public ModelStateException(string message) : base(message) { }
    }
}
