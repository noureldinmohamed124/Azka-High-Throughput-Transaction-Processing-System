using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Exceptions
{
    public sealed class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message) { }
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}
