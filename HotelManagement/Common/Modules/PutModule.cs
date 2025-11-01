using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Routing;
using HotelManagement.Common.Responses.EndpointResults;

namespace HotelManagement.Common.Modules
{
    public abstract class PutModule<TRequest> : BaseModule<TRequest>
    {
        protected override abstract Func<TRequest, IMediator, Task<IResult>> Handler { get; }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut(Route, async (TRequest request, HttpContext context) =>
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
            });
        }
    }
}
