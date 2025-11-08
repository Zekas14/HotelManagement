using FluentValidation;
using HotelManagement.Features.RoomManagement.Rooms;
using HotelManagement.Features.RoomManagement.Facilities;

namespace HotelManagement.Features.RoomManagement
{
    public static class RoomManagementDI
    {
        public static IServiceCollection AddRoomManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<AddRoomCommand>, AddRoomCommandValidator>();
            services.AddScoped<IValidator<UpdateRoomCommand>, UpdateRoomCommandValidator>();
            services.AddScoped<IValidator<DeleteRoomCommand>, DeleteRoomCommandValidator>();

            // Facilities
            services.AddScoped<IValidator<AddFacillityCommand>, AddFacillityCommandValidator>();
            services.AddScoped<IValidator<UpdateFacilityCommand>, UpdateFacilityCommandValidator>();

            return services;
        }
    }
}
