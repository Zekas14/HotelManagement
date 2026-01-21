using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace HotelManagement.Features.ReservationManagement.Reservations.Queries.ViewReservation
{
    public record ViewReservationQuery(int ReservationId) : IRequest<RequestResult<ViewReservationResponseDto>>;
   
    #region ResponseDto
    public class ViewReservationResponseDto     {
        public string? Guest { get; init; }
        public int RoomId { get; init; }
        public string? CheckInDate { get; init; }
        public string? CheckOutDate { get; init; }
        public string? TotalPrice { get; init; }   
    }
    #endregion

    #region Handler
    public class ViewReservationQueryHandler(IGenericRepository<Reservation> repository, IMediator mediator) : IRequestHandler<ViewReservationQuery, RequestResult<ViewReservationResponseDto>>
    {
        private readonly IGenericRepository<Reservation> _repository = repository;
        private readonly IMediator _mediator = mediator;
        public async Task<RequestResult<ViewReservationResponseDto>> Handle(ViewReservationQuery request, CancellationToken cancellationToken)
        {
            var reservation = await _repository.Get(r=>r.Id==request.ReservationId).Select(r=>
            new ViewReservationResponseDto
            {
                Guest = r.Guest.User.Username,
                RoomId = r.Room.RoomNumber,
                CheckInDate = r.CheckInDate.ToString(Constants.DateFormat),
                CheckOutDate = r.CheckOutDate.ToString(Constants.DateFormat),
                TotalPrice = $"{r.TotalPrice:c2}"
            }).FirstOrDefaultAsync(cancellationToken);
            if (reservation is null)
            {
                return RequestResult<ViewReservationResponseDto>.Failure(ErrorCode.NotFound,"Reservation Not Found");
            }

            return  RequestResult<ViewReservationResponseDto>.Success(reservation,"Returned Successfully");
        }

    }

    #endregion
}
