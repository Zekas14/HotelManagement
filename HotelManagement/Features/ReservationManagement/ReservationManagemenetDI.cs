using FluentValidation;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.CancelReservation;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.EditReservation;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation;

namespace HotelManagement.Features.ReservationManagement
{
    public static class ReservationManagemenetDI
    {
        public static IServiceCollection AddReservationManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<MakeReservationCommand>, Validator>();
            services.AddScoped<IValidator<CancelReservationCommand>, CancelReservationValidator>();
            services.AddScoped<IValidator<EditReservationCommand>, EditReservationCommandValidator>();
            return services;
        }
    }

}
