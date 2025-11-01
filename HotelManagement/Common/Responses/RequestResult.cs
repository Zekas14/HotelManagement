namespace HotelManagement.Common.Responses
{
    public class RequestResult<T>(T data, bool isSuccess, string message)
    {
        public T Data { get; } = data;
        public bool IsSuccess { get; } = isSuccess;
        public string Message { get; } = message;
       
        public static RequestResult<T> Success(T data, string message = "")
        {
            return new RequestResult<T>(data : data, isSuccess :true, message);
        }

        
        public static RequestResult<T> Failure( string message)
        {
            return new RequestResult<T>(data :default!, isSuccess: false, message: message);
        }
    }
}
