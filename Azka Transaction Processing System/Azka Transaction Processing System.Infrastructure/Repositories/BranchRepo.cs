using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Modules.Branches.GetAllBranches;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IReadOnlyList<BranchResponse>> GetAllBranchesAsync()
        {
            return await _context.Branches
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new BranchResponse
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
        }
                
    }
}
