using FluentValidation;
using HotelManagement.Features.RoomManagement.Rooms;

namespace HotelManagement.Features.RoomManagement
{
    public static class RoomManagementDI
    {
        public static IServiceCollection AddRoomManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<AddRoomCommand>, AddRoomCommandValidator>();
            services.AddScoped<IValidator<UpdateRoomCommand>, UpdateRoomCommandValidator>();
            services.AddScoped<IValidator<DeleteRoomCommand>, DeleteRoomCommandValidator>();

            return services;
        }
    }
}
