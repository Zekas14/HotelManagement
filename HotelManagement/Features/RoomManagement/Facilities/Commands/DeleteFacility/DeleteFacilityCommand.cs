using FluentValidation;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Domain.Models;
using MediatR;
using HotelManagement.Features.Common.Responses;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.DeleteFacility
{
    #region Command
    public record DeleteFacilityCommand(int Id) : IRequest<RequestResult<bool>>;
    public class DeleteFacilityCommandValidator : AbstractValidator<DeleteFacilityCommand>
    {
        public DeleteFacilityCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    #endregion

    #region Handler
    public class DeleteFacilityCommandHandler(IGenericRepository<Facility> repository , IMediator mediator) : IRequestHandler<DeleteFacilityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Facility> _repository = repository;
        private readonly IMediator _mediator = mediator;

        public async Task<RequestResult<bool>> Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
        {
            var isFacilityExistsResult = await _mediator.Send(new EntityExistsQuery<Facility>(request.Id), cancellationToken);
            if (!isFacilityExistsResult.IsSuccess)
            {
                return RequestResult<bool>.Failure(isFacilityExistsResult.ErrorCode,isFacilityExistsResult.Message);
            }
            _repository.Delete(request.Id);
            await _repository.SaveChangesAsync();
            return RequestResult<bool>.Success(true, "Facility deleted successfully");
        }
    }

    #endregion
}
