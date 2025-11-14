using FluentValidation;
using HotelManagement.Features.ReservationManagement.Reservations.Commands;

namespace HotelManagement.Features.ReservationManagement
{
    public static class ReservationManagemenetDI
    {
        public static IServiceCollection AddReservationManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<MakeReservationCommmand>, MakeReservationValidator>();
            return services;
        }
    }

}
