using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.Common.Queries
{
    public record EntityExistsQuery<Entity>(int Id) : IRequest<RequestResult<Entity>> where Entity : BaseModel;
    public class EntityExistsQueryHandler<Entity>(IGenericRepository<Entity> repository) : IRequestHandler<EntityExistsQuery<Entity>, RequestResult<Entity>> where Entity : BaseModel
    {
        private readonly IGenericRepository<Entity> _repository = repository;
        public async Task<RequestResult<Entity>> Handle(EntityExistsQuery<Entity> request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.Id);
            if (entity == null)

            {
                return RequestResult<Entity>.Failure(ErrorCode.NotFound, $"{typeof(Entity).Name} does not exist.");
            }
            return RequestResult<Entity>.Success(entity,$"{request.Id} exists.");
        }
    }

}
