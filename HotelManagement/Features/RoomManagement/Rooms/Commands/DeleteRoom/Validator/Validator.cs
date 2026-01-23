using FluentValidation;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.DeleteRoom
{
    public class DeleteRoomCommandValidator: AbstractValidator<DeleteRoomCommand>
    {
        public DeleteRoomCommandValidator()
        {
            RuleFor(x=>x.RoomID).NotEmpty();
        }
    } 
}
