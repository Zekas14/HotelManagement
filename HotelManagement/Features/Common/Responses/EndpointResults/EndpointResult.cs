using System.Text.Json;

namespace HotelManagement.Features.Common.Responses.EndpointResults
{
    public abstract  class EndpointResult<T> : IResult
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public ErrorCode ErrorCode { get; set; }
        public string Message { get; set; }
        

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            var response = httpContext.Response;
            response.ContentType = "application/json";

            response.StatusCode = IsSuccess ? StatusCodes.Status200OK : GetStatusCode(ErrorCode);

            var result = JsonSerializer.Serialize(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await response.WriteAsync(result);
        }
        private int GetStatusCode(ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.NotFound => StatusCodes.Status404NotFound,
                ErrorCode.EmailAlreadyConfirmed => StatusCodes.Status409Conflict,
                ErrorCode.UnAuthorized => StatusCodes.Status401Unauthorized,
                ErrorCode.Forbidden => StatusCodes.Status403Forbidden,
                ErrorCode.InternalServerError => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status400BadRequest
            };
        }

    }
        public enum ErrorCode
    {
        None = 200,
        InvalidInput = 2,
        BadRequest = 400,
        UnAuthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        InternalServerError = 405,
        EmailAlreadyConfirmed = 409,
        InternalError = 410,
        LimitReached = 429,
    }
}
