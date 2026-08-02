using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Branches.GetAllBranches
{
    public class GetAllBranchesUseCase
    {
        private readonly IBranchRepo _branchRepo;

        public GetAllBranchesUseCase(IBranchRepo branchRepo)
        {
            _branchRepo = branchRepo;
        }

        public async Task<IReadOnlyList<BranchResponse>> ExecuteAsync()
        {
            return await _branchRepo.GetAllBranchesAsync();
        }

    }
}
