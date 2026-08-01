using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Services
{
    public interface ICurrentUserService
    {
        int UserId { get; }
    }
}
