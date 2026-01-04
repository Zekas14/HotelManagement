using FluentValidation;
using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Infrastructure.Data;
using MediatR;
using System.Reflection.Metadata;
using HotelManagement.Features.Common.Responses;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.AddFacillity
{
    #region Command & Validator
    public record AddFacillityCommand(string Name): IRequest<RequestResult<bool>>;
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
    #endregion

    #region Command Handler
    public class AddFacillityCommandHandler(IGenericRepository<Facility> repository) : IRequestHandler<AddFacillityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Facility> repository = repository;

        public  Task<RequestResult<bool>> Handle(AddFacillityCommand request, CancellationToken cancellationToken)
        {
            var facility = new Facility
            {
                Name = request.Name
            };
             repository.Add(facility);
            repository.SaveChanges();
          return Task.FromResult( RequestResult<bool>.Success(true,"Facility Added Successfully"));
        }
    }

    #endregion

}
