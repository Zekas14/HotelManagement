using FluentValidation;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.CancelReservation.Commands;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.EditReservation;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation.Commands;

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
