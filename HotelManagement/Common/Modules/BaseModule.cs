using Carter;

namespace HotelManagement.Common.Modules
{
    public abstract class BaseModule<TRequest> : ICarterModule
    {

        protected abstract string Route { get; }
        protected abstract  Delegate Handler { get; }
        public abstract void AddRoutes(IEndpointRouteBuilder app);
        
    }
}
