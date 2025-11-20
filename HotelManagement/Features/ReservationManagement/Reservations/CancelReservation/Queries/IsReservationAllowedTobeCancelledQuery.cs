using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.CancelReservation.Queries
{
    public record IsReservationAllowedTobeCancelledQuery(int ReservationId) : IRequest<RequestResult<bool>>;
    public class iIsReservationAllowedTobeCancelledQueryHandler(IGenericRepository<Reservation> repository)
     : IRequestHandler<IsReservationAllowedTobeCancelledQuery, RequestResult<bool>>{

        public async Task<RequestResult<bool>> Handle(IsReservationAllowedTobeCancelledQuery request, CancellationToken cancellationToken)
        {
            var reservation =  repository.GetById(request.ReservationId);
            if(reservation == null)
            {
                return RequestResult<bool>.Failure(ErrorCode.NotFound, "Reservation not found");
            }
            if(reservation.CheckInDate > DateTime.Now|| reservation.CheckInDate - DateTime.Now.Date > TimeSpan.FromDays(2))
            {
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, "Reservation cannot be cancelled more than 2 days before check-in");
            }
            return RequestResult<bool>.Success(true, "Reservation can be cancelled");
        }
    }
    
}