using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Application.Modules.Branches.GetAllBranches;
using Azka_Transaction_Processing_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Repositories
{
    public interface IBranchRepo : IGenericRepo<Branch>
    {
        Task<IReadOnlyList<BranchResponse>> GetAllBranchesAsync();
    }
}
