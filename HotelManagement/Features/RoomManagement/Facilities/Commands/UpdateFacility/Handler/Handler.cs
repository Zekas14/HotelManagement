using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Features.Common.Responses;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.UpdateFacility
{
    #region Command
    public record UpdateFacilityCommand(int Id, string Name) : IRequest<RequestResult<bool>>;

    #endregion

    #region Handler
    public class UpdateFacilityCommandHandler(IGenericRepository<Facility> repository, IMediator mediator) : IRequestHandler<UpdateFacilityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Facility> _repository = repository;
        private readonly IMediator mediator = mediator;

        public async Task<RequestResult<bool>> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
        {
           var isFacilityExists=  await mediator.Send(new EntityExistsQuery<Facility>(request.Id), cancellationToken);
            if (!isFacilityExists.IsSuccess)
            {
                return RequestResult<bool>.Failure(isFacilityExists.ErrorCode,isFacilityExists.Message);
            }
            _repository.SaveInclude(isFacilityExists.Data, nameof(Facility.Name));
            await _repository.SaveChangesAsync();
            return RequestResult<bool>.Success(true, "Facility updated successfully");
        }
    }

    #endregion
}
