using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Exceptions
{
    public sealed class ValidationException : AppException
    {
        public Dictionary<string, string[]> Errors { get; set; }

        public ValidationException(Dictionary<string, string[]> errors, string message = "Validation failed") : base(message)
        {
            Errors = errors;
        }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
            Errors.Add("Error 1", [message]);
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}
