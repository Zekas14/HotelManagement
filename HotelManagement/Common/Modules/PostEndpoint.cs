using FluentValidation;
using HotelManagement.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Common.Modules
{
    public abstract class PostEndpoint<TRequest, TResponseType>(IValidator<TRequest> validator ,IMediator mediator) : BaseEndpoint<TRequest, TResponseType>(mediator)
    {
        protected readonly IValidator<TRequest> _validator = validator;

        protected async Task<EndpointResult<TResponseType>> Validate(TRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return FailureEndpointResult<TResponseType>.BadRequest(errors);
            }
            return new SuccessEndpointResult<TResponseType>(default!) ;
        }
        public override void Configure()
        {
            Post(GetRoute());
        }
    }
}

