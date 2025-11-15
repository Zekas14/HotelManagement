using FluentValidation;
using HotelManagement.Features.RoomManagement.Facilities;
using HotelManagement.Features.RoomManagement.RoomFacilities;
using HotelManagement.Features.RoomManagement.Rooms.Commands;

namespace HotelManagement.Features.RoomManagement
{
    public static class RoomManagementDI
    {
        public static IServiceCollection AddRoomManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<AddRoomCommand>, AddRoomCommandValidator>();
            services.AddScoped<IValidator<UpdateRoomCommand>, UpdateRoomCommandValidator>();
            services.AddScoped<IValidator<DeleteRoomCommand>, DeleteRoomCommandValidator>();

           
            services.AddScoped<IValidator<AddFacillityCommand>, AddFacillityCommandValidator>();
            services.AddScoped<IValidator<UpdateFacilityCommand>, UpdateFacilityCommandValidator>();
            services.AddScoped<IValidator<DeleteFacilityCommand>, DeleteFacilityCommandValidator>();
            services.AddScoped<IValidator<AddRoomFacilityCommand>, AddRoomFacilityCommandValidator>();

            return services;
        }
    }
}
