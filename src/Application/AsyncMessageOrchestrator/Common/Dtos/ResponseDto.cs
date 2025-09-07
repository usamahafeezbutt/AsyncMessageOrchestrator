
namespace AsyncMessageOrchestrator.Application.Dtos
{
    public class ResponseDto<T>
    {
        public ResponseDto(
            bool success,
            string message,
            T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
