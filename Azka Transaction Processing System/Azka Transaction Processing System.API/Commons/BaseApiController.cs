using Microsoft.AspNetCore.Mvc;

namespace Azka_Transaction_Processing_System.API.Commons
{
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult OkResponse<T>(T? data, string? message = null)
            => Ok(ApiResponse<T>.Ok(data, message));

        protected IActionResult OkResponse(string message)
            => Ok(ApiResponse<object>.Ok(null, message));

        protected IActionResult CreatedResponse<T>(T? data, string? message = null) 
            => StatusCode(StatusCodes.Status201Created, ApiResponse<T>.Ok(data, message));

        protected IActionResult NoContentResponse()
            => NoContent();
    }
}
