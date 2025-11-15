using FastEndpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.Common.Endpoints
{
    public abstract class GetEndpoint<TResponseType>(IMediator mediator) : EndpointWithoutRequest<EndpointResult<TResponseType>>
    {
        protected readonly IMediator _mediator = mediator;
        protected abstract string GetRoute();
        override public void Configure()
        {
            Get(GetRoute());
            Throttle(hitLimit: 5, durationSeconds: 60);
            AllowAnonymous();
        }
    }
}
