using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Modules.Branches.GetAllBranches
{
    public class BranchResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
