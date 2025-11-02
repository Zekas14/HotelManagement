using FluentValidation;
using HotelManagement.Features.RoomManagement.Rooms;

namespace HotelManagement.Features.RoomManagement
{
    public static class RoomManagementDI
    {
        public static IServiceCollection AddRoomManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<AddRoomDto>, AddRoomCommandValidator>();

            return services;
        }
    }
}
