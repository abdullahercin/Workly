namespace Workly.API.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        // Başarılı Response
        public static ApiResponse<T> Success(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message ?? "İşlem başarılı",
            };
        }

        // Başarısız Response
        public static ApiResponse<T> Fail(string errorMessage, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = errorMessage,
                Errors = errors,
            };
        }

        // Hızlı kullanım için implicit operator
        public static implicit operator ApiResponse<T>(T data) => Success(data);
    }
}
