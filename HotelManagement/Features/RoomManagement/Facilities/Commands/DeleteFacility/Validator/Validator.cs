using FluentValidation;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.DeleteFacility
{
    public class DeleteFacilityCommandValidator : AbstractValidator<DeleteFacilityCommand>
    {
        public DeleteFacilityCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

}
