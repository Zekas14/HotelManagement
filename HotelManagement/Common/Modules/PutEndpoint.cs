using FluentValidation;
using MediatR;

namespace HotelManagement.Common.Modules
{
    public abstract class PutEndpoint<TRequest, TResponseType>(IMediator mediator , IValidator<TRequest> validator) : BaseEndpoint<TRequest,TResponseType>(mediator, validator)
    {
        
        public override void Configure()
        {
            Put(GetRoute());
            AllowAnonymous();
        }

        
    }
}

