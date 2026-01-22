using FluentValidation;
using HotelManagement.Features.RoomManagement.Facilities.Commands.AddFacillity;
using HotelManagement.Features.RoomManagement.Facilities.Commands.DeleteFacility;
using HotelManagement.Features.RoomManagement.Facilities.Commands.UpdateFacility;
using HotelManagement.Features.RoomManagement.RoomFacilities.Commands.AddRoomFacility;
using HotelManagement.Features.RoomManagement.Rooms.Commands.AddRoom;
using HotelManagement.Features.RoomManagement.Rooms.Commands.DeleteRoom;
using HotelManagement.Features.RoomManagement.Rooms.Commands.UpdateRoom;

namespace HotelManagement.Features.RoomManagement
{
    public static class RoomManagementDI
    {
        public static IServiceCollection AddRoomManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<AddRoomCommand>, AddRoomValidator>();
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
