using HotelManagement.Features.Common.Responses.EndpointResults;

namespace HotelManagement.Features.Common.Responses
{
    public class RequestResult<T>(T data, bool isSuccess, ErrorCode errorCode,string message)
    {
        public T Data { get; } = data;
        public bool IsSuccess { get; } = isSuccess;
        public string Message { get; } = message;
        public ErrorCode ErrorCode { get; } = errorCode; 
        public static RequestResult<T> Success(T data, string message = "")
        {
            return new RequestResult<T>(data : data, isSuccess :true,ErrorCode.None, message);
        }

        
        public static RequestResult<T> Failure( ErrorCode errorCode,string message)
        {
            return new RequestResult<T>(data :default!, isSuccess: false,errorCode, message: message);
        }
    }
}
