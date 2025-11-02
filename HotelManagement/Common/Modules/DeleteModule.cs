using MediatR;

namespace HotelManagement.Common.Modules
{
    public abstract class DeleteModule<TRequest>(IMediator mediator) : BaseEndpoint<TRequest, bool>(mediator)
    {
        public override void Configure()
        {
            Delete(GetRoute());
            AllowAnonymous();
        }
    }
}

