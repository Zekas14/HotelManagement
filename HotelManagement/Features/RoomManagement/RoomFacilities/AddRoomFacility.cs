using FluentValidation;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.RoomFacilities
{
    #region Command & Validator
    public record AddRoomFacilityCommand(int RoomId, int FacilityId) : IRequest<RequestResult<bool>>;

    public class AddRoomFacilityCommandValidator : AbstractValidator<AddRoomFacilityCommand>
    {
        private readonly ApplicationDbContext context;

        public AddRoomFacilityCommandValidator(ApplicationDbContext context)
        {
            this.context = context;
            RuleFor(x => x.RoomId).GreaterThan(0);
            RuleFor(x => x.FacilityId).GreaterThan(0);
            RuleFor(x => x.RoomId).Must(RoomExists).WithMessage("Room does not exist.");
          //  RuleFor(x => x.FacilityId).Must(FacilityExists).WithMessage("Facility does not exist.");
        }
        private bool RoomExists(int roomId)
        {
            return context.Rooms.Any(r => r.Id == roomId);
        }
        private bool FacilityExists(int facilityId)
        {
            return context.Facilities.Any(f => f.Id == facilityId);
        }
    }
    #endregion

    #region Handler
    public class AddRoomFacilityCommandHandler(IGenericRepository<RoomFacility> repository, IMemoryCache cache) : IRequestHandler<AddRoomFacilityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<RoomFacility> _repository = repository;
        private readonly IMemoryCache _cache = cache;

        public async Task<RequestResult<bool>> Handle(AddRoomFacilityCommand request, CancellationToken cancellationToken)
        {
            var IsFacilityAssignedToRoom= _repository.GetAll()
                .Any(rf => rf.RoomId == request.RoomId && rf.FacilityId == request.FacilityId);
                
            if (IsFacilityAssignedToRoom)
                return RequestResult<bool>.Failure(ErrorCode.FacilityAlreadyAssignedToRoom, "Facility already assigned to room.");
            _repository.Add(new RoomFacility
            {
                RoomId = request.RoomId,
                FacilityId = request.FacilityId
            });
            _repository.SaveChanges();
            _cache.Remove("rooms");
            
            return RequestResult<bool>.Success(true, "Facility assigned to room successfully");
        }
    }
    #endregion

    #region Endpoint
    public class AddRoomFacilityEndPoint(IValidator<AddRoomFacilityCommand> validator, IMediator mediator) : PostEndpoint<AddRoomFacilityCommand, bool>(validator, mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/roomfacility/add";

        public override async Task HandleAsync(AddRoomFacilityCommand req, CancellationToken ct)
        {
            var validationResult = await Validate(req);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);
                return;
            }

            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }
    #endregion
}
