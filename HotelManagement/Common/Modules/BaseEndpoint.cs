using FastEndpoints;
using HotelManagement.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Common.Modules
{
    public abstract class BaseEndpoint<TRequest, TResponseType>(IMediator mediator) : Endpoint<TRequest, EndpointResult<TResponseType>>
    {
        protected readonly IMediator mediator = mediator;

        protected abstract string GetRoute();

    }
}

