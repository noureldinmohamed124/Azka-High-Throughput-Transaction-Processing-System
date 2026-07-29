using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Exceptions
{
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message) { }
        public abstract HttpStatusCode StatusCode { get; }
    }
}
