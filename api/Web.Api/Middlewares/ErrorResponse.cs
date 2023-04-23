namespace Web.Api.Middlewares
{
    public sealed class ErrorResponse
    {
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public List<string> Parameters { get; set; }

        public ErrorResponse(string message)
        {
            Message = message;
        }
    }
}