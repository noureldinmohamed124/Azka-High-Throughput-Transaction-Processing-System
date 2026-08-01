using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Repositories
{
    public interface ICustomerRepo : IGenericRepo<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email);

        Task<Customer?> GetByPhoneAsync(string phone);

        Task<bool> ExistsAsync(int customerId);
    }
}
