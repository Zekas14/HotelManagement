using FluentValidation;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.UpdateFacility
{
    public class UpdateFacilityCommandValidator : AbstractValidator<UpdateFacilityCommand>
    {

        private readonly IGenericRepository<Facility> repository;

        public UpdateFacilityCommandValidator(IGenericRepository<Facility> repository)
        {
            this.repository = repository;
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .Must(BeUniqueName)
                .WithMessage("Facility name already used");
        }

        private bool BeUniqueName(string name)
        {
            return !repository.GetAll().Any(f => f.Name == name);
        }
    }
}
