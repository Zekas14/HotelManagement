using Carter;
using FluentValidation;
using MediatR;
using HotelManagement.Common.Responses.EndpointResults;

namespace HotelManagement.Common.Modules
{
    public abstract class PostModule<TRequest> : ICarterModule
    {
        protected abstract string Route { get; }

        protected abstract Func<TRequest, IMediator, Task<IResult>> Handler { get; }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost(Route, GetEndpointDelegate());
        }

        private Func<TRequest, HttpContext, Task<IResult>> GetEndpointDelegate()
        {
            return async (TRequest request, HttpContext context) =>
            {
                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var validator = context.RequestServices.GetService<IValidator<TRequest>>();

                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                        return FailureEndpointResult<string>.BadRequest(errors);
                    }
                }

                return await Handler(request, mediator);
            };
        }
    }
}
