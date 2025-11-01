namespace HotelManagement.Common.Responses.EndpointResults
{
    
        public class SuccessEndpointResult<T> : EndpointResult<T>
        {
            public SuccessEndpointResult(T data, string message = "")
            {
                Data = data;
                IsSuccess = true;
                Message = message;
                ErrorCode = ErrorCode.None;
            }
        }
    
}
