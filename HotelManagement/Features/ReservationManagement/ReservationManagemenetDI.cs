using FluentValidation;
using HotelManagement.Features.ReservationManagement.Reservations.CancelReservation.Commands;
using HotelManagement.Features.ReservationManagement.Reservations.EditReservation;
using HotelManagement.Features.ReservationManagement.Reservations.MakeReservation.Commands;

namespace HotelManagement.Features.ReservationManagement
{
    public static class ReservationManagemenetDI
    {
        public static IServiceCollection AddReservationManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<MakeReservationCommmand>, MakeReservationValidator>();
            services.AddScoped<IValidator<CancelReservationCommand>, CancelReservationValidator>();
            services.AddScoped<IValidator<EditReservationCommand>, EditReservationCommandValidator>();
            return services;
        }
    }

}
