namespace HotelManagement.Common.Responses.EndpointResults
{
    public class FailureEndpointResult<T> : EndpointResult<T>
    {
        public FailureEndpointResult(ErrorCode errorCode, string message = "")
        {
            Data = default;
            IsSuccess = false;
            Message = message;
            ErrorCode = errorCode;
        }
        public static FailureEndpointResult<T> NotFound(string message = "")
        {
            return new FailureEndpointResult<T>(ErrorCode.NotFound, message ?? ErrorCode.NotFound.ToString());
        }
        public static FailureEndpointResult<T> UnAuthorized(string message = "")
        {
            return new FailureEndpointResult<T>(ErrorCode.UnAuthorized, message ?? ErrorCode.UnAuthorized.ToString());
        }
        public static FailureEndpointResult<T> Forbidden(string message = "")
        {
            return new FailureEndpointResult<T>(ErrorCode.Forbidden, message ?? ErrorCode.Forbidden.ToString());
        }
        public static FailureEndpointResult<T> InternalServerError(string message = "")
        {
            return new FailureEndpointResult<T>(ErrorCode.InternalServerError, message ?? ErrorCode.InternalServerError.ToString());
        }
        public static FailureEndpointResult<T> BadRequest(string message = "")
        {
            return new FailureEndpointResult<T>(ErrorCode.BadRequest, message ?? ErrorCode.BadRequest.ToString());
        }
    }
}
