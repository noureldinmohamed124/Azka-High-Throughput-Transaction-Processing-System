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
    public class BranchRepo : GenericRepo<Branch>, IBranchRepo
    {
        public BranchRepo(TPSDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(int branchId)
        {
            throw new NotImplementedException();
        }

        public Task<Branch?> GetByCodeAsync(string code)
        {
            throw new NotImplementedException();
        }
    }
}
