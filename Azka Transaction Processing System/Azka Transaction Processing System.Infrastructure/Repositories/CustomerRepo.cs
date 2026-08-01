using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Infrastructure.Presistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Repositories
{
    public class CustomerRepo : GenericRepo<Customer>, ICustomerRepo
    {
        public CustomerRepo(TPSDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(int customerId)
        {
            throw new NotImplementedException();
        }

        public Task<Customer?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Customer?> GetByPhoneAsync(string phone)
        {
            throw new NotImplementedException();
        }
    }
}
