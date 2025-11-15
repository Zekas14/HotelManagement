using FastEndpoints;
using FluentValidation;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Features.Common.Endpoints
{
    public abstract class BaseEndpoint<TRequest, TResponseType>(IMediator mediator, IValidator<TRequest> validator) : Endpoint<TRequest, EndpointResult<TResponseType>>
    {
        protected readonly IMediator mediator = mediator;

        protected readonly IValidator<TRequest> _validator = validator;
        protected abstract string GetRoute();

        protected async Task<EndpointResult<TResponseType>> Validate(TRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return FailureEndpointResult<TResponseType>.BadRequest(errors);
            }
            return new SuccessEndpointResult<TResponseType>(default!);
        }

    }
}

