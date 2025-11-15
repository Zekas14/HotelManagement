using HotelManagement.Common.Responses.EndpointResults;

namespace HotelManagement.Infrastructure.Middlewares
{
    public class GlobalErrorHandlerMiddleware

    {
        private readonly RequestDelegate _nextAction;
        public GlobalErrorHandlerMiddleware(RequestDelegate nextAction,
            ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _nextAction = nextAction;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _nextAction(context);
            }
            catch (Exception ex)
            {
                
                var response = FailureEndpointResult<bool>.BadRequest(message: $"{ex.InnerException}");
                await context.Response.WriteAsJsonAsync(response);
            }

        }
    }
}
