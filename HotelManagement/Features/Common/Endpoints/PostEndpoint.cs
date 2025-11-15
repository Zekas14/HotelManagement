using FluentValidation;
using MediatR;

namespace HotelManagement.Features.Common.Endpoints
{
    public abstract class PostEndpoint<TRequest, TResponseType>(IValidator<TRequest> validator ,IMediator mediator) : BaseEndpoint<TRequest, TResponseType>(mediator, validator)
    {
        
        public override void Configure()
        {
            Post(GetRoute());
            Throttle(hitLimit:10,durationSeconds:60 );
            AllowAnonymous();
        }
    }
}

