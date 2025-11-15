using FastEndpoints;
using FluentValidation;
using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Domain.Models;
using HotelManagement.Features.ReservationManagement.Reservations.Events;
using HotelManagement.Features.ReservationManagement.Reservations.Queries;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands
{
    #region Command
    public record MakeReservationCommmand   (
        DateTime CheckInDate,
        DateTime CheckOutDate,
        int RoomId,
        int GuestId,
        int NumberOfGuests,
        decimal PricePerNight
    ) : IRequest<RequestResult<bool>>;
    #endregion

    #region  Validator

        public class MakeReservationValidator : AbstractValidator<MakeReservationCommmand>
        {
            private readonly IGenericRepository<Room> repository;
            public MakeReservationValidator(IGenericRepository<Room> repository)
            {
                this.repository = repository;
                RuleFor(x => x.CheckInDate)
                    .LessThan(x => x.CheckOutDate)
                    .WithMessage("Check-in date must be before check-out date.");
                RuleFor(x => x.CheckInDate)
                    .GreaterThanOrEqualTo(DateTime.Now)
                    .WithMessage("Chacke-in Date is Passed");
                RuleFor(x => x.NumberOfGuests)
                    .GreaterThan(0)
                    .WithMessage("Number of guests must be greater than zero.");
                RuleFor(x => x.PricePerNight)
                    .GreaterThan(0)
                    .WithMessage("Price per night must be greater than zero.");
                RuleFor(x => x.CheckOutDate)
                    .GreaterThan(x => x.CheckInDate)
                    .WithMessage("Check-out date must be after check-in date.");
                RuleFor(x => x.CheckOutDate)
                  .GreaterThan(DateTime.Now)
                  .WithMessage("Chacke-Out Date Must be in the Future");
                RuleFor(x => x.RoomId)
                    .Must(RoomExists)
                    .WithMessage("Room does not exist.");
            }
            private bool RoomExists(int roomId)
            {
                return repository.GetAll().Any(r => r.Id == roomId);
            }
        }
    #endregion

    #region Handler

    public class MakeReservationHandler(IGenericRepository<Reservation> repository, IMediator mediator) : IRequestHandler<MakeReservationCommmand, RequestResult<bool>>
    {
            private readonly IGenericRepository<Reservation> repository = repository;
            private readonly IMediator mediator = mediator;

            public async Task<RequestResult<bool>> Handle(MakeReservationCommmand request, CancellationToken cancellationToken)
            {
            var isRoomAvailable = await mediator.Send(new CheckRoomAvailabilityQuery(request.RoomId, request.CheckInDate, request.CheckOutDate), cancellationToken);
            if (!isRoomAvailable)
            {
                return RequestResult<bool>.Failure("Room is not available for the selected dates.");
            }
            var reservation = new Reservation
            {
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                RoomId = request.RoomId,
                GuestId = request.GuestId,
                NumberOfGuests = request.NumberOfGuests,
                TotalPrice = ((request.CheckOutDate - request.CheckInDate).Days) * request.PricePerNight
            };
            repository.Add(reservation);
            await mediator.Publish(new MakeReservationEvent(request.RoomId));
            return RequestResult<bool>.Success(true);
        }

    }

    #endregion

    #region Endpoint
    public class MakeReservationEndpoint(IMediator mediator ,IValidator<MakeReservationCommmand> validator) : PostEndpoint<MakeReservationCommmand, bool>(validator,mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reservations/makeReservation";
        public override async Task HandleAsync(MakeReservationCommmand req, CancellationToken ct)
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
 