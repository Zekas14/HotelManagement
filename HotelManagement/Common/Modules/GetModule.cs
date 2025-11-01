using MediatR;

namespace HotelManagement.Common.Modules
{
    public abstract class GetModule<TRequest> : BaseModule<TRequest>
    {
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(Route, Handler);
        }
    }
}
