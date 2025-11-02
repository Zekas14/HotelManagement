using FastEndpoints;
using HotelManagement.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Common.Modules
{
    public abstract class GetEndpoint<TResponseType>(IMediator mediator) : EndpointWithoutRequest<EndpointResult<TResponseType>>
    {
        protected readonly IMediator _mediator = mediator;
        protected abstract string GetRoute();
        override public void Configure()
        {
            Get(GetRoute());
            AllowAnonymous();
        }
    }
}
