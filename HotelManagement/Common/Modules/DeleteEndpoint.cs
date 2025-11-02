using FluentValidation;
using MediatR;

namespace HotelManagement.Common.Modules
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

