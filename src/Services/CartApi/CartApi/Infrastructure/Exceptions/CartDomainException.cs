using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CartApi.Infrastructure.Exceptions
{
    public class CartDomainException : Exception
    {
        public CartDomainException()
        {
        }

        public CartDomainException(string message) : base(message)
        {
        }

        public CartDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CartDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
