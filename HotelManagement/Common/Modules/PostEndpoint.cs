using FluentValidation;
using HotelManagement.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Common.Modules
{
    public abstract class PostEndpoint<TRequest, TResponseType>(IValidator<TRequest> validator ,IMediator mediator) : BaseEndpoint<TRequest, TResponseType>(mediator, validator)
    {
        
        public override void Configure()
        {
            Post(GetRoute());
            AllowAnonymous();
        }
    }
}

