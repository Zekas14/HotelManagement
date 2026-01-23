using FluentValidation;
using HotelManagement.Infrastructure.Data;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.RoomManagement.RoomFacilities.Commands.AddRoomFacility
{
    public class AddRoomFacilityCommandValidator : AbstractValidator<AddRoomFacilityCommand>
    {
        private readonly ApplicationDbContext context;

        public AddRoomFacilityCommandValidator(ApplicationDbContext context)
        {
            this.context = context;
            RuleFor(x => x.RoomId).GreaterThan(0);
            RuleFor(x => x.FacilityId).GreaterThan(0);
            RuleFor(x => x.RoomId).Must(RoomExists).WithMessage("Room does not exist.");
            RuleFor(x => x.FacilityId).Must(FacilityExists).WithMessage("Facility does not exist.");
        }
        private bool RoomExists(int roomId)
        {
            return context.Rooms.Any(r => r.Id == roomId);
        }
        private bool FacilityExists(int facilityId)
        {
            return context.Facilities.Any(f => f.Id == facilityId);
        }
    }

}
