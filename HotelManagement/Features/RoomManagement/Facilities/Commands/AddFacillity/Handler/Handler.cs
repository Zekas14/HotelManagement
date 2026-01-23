using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Infrastructure.Data;
using MediatR;
using System.Reflection.Metadata;
using HotelManagement.Features.Common.Responses;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.AddFacillity
{
    #region Command
    public record AddFacillityCommand(string Name): IRequest<RequestResult<bool>>;
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
