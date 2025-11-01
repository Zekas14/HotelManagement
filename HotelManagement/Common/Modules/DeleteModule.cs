using MediatR;

namespace HotelManagement.Common.Modules
{
    public abstract class DeleteModule<TRequest>: BaseModule<TRequest>
    {
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete(Route, Handler);
        }
    }
}
