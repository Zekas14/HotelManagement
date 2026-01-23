using FluentValidation;
using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Repositories;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.AddFacillity
{
    public class AddFacillityCommandValidator : AbstractValidator<AddFacillityCommand>
    {
        IGenericRepository<Facility> repository;

        public AddFacillityCommandValidator(IGenericRepository<Facility> repository)
        {
            this.repository = repository;
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Facility name is required.")
                .MaximumLength(100).WithMessage("Facility name must not exceed 100 characters.")
                .Must(BeUniqueName).WithMessage("Faclity Name already Used");

        }
        private bool BeUniqueName(string name)
        {
            return !repository.GetAll().Any(f => f.Name == name);
        }
    }


}
