using Azka_Transaction_Processing_System.API.Commons;
using Azka_Transaction_Processing_System.Application.Modules.Branches.GetAllBranches;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Azka_Transaction_Processing_System.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : BaseApiController
    {
        private readonly GetAllBranchesUseCase _getAllBranchesUseCase;

        public BranchesController(GetAllBranchesUseCase getBranchesUseCase)
        {
            _getAllBranchesUseCase = getBranchesUseCase;
        }


        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _getAllBranchesUseCase.ExecuteAsync();
            return OkResponse(branches);
        }

    }
}
