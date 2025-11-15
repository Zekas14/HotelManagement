using FluentValidation;
using MediatR;

namespace HotelManagement.Features.Common.Endpoints
{
    public abstract class DeleteEndpoint<TRequest>(IMediator mediator,IValidator<TRequest> validator) : BaseEndpoint<TRequest, bool>(mediator, validator)
    {
        public override void Configure()
        {
            Delete(GetRoute());
            AllowAnonymous();
        }
    }
}

