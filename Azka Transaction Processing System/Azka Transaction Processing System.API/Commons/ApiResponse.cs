using System.Text.Json.Serialization;

namespace Azka_Transaction_Processing_System.API.Commons
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Errors { get; init; }

        public static ApiResponse<T> Ok(T? data, string? message = null)
            => new ApiResponse<T>() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null)
            => new ApiResponse<T>() { Success = false, Message = message, Errors = errors };


    }
}
