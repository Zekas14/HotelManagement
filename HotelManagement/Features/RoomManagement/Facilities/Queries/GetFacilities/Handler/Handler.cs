using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HotelManagement.Features.RoomManagement.Facilities.Queries.GetFacilities
{
    #region Query
    public record GetFacilitiesQuery : IRequest<RequestResult<IReadOnlyList<GetFacilitiesResponseDto>>>;

    #endregion
    #region Response
    #endregion

    #region Handler
    public class GetFacilitiesHandler(IGenericRepository<Facility> repository) : IRequestHandler<GetFacilitiesQuery, RequestResult<IReadOnlyList<GetFacilitiesResponseDto>>>
    {
        private readonly IGenericRepository<Facility> _repository = repository;

        public Task<RequestResult<IReadOnlyList<GetFacilitiesResponseDto>>> Handle(GetFacilitiesQuery request, CancellationToken cancellationToken)
        {
            var data = _repository.GetAll()
                .Select(e => new GetFacilitiesResponseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    CreatedDate = e.CreatedAt.ToString(Constants.DateTimeFormat)
                });

            if (!data.Any())
            {
                return Task.FromResult(RequestResult<IReadOnlyList<GetFacilitiesResponseDto>>.Failure(ErrorCode.NotFound, "No facilities found"));
            }

            return Task.FromResult(RequestResult<IReadOnlyList<GetFacilitiesResponseDto>>.Success(data.ToList(), "Facilities retrieved successfully"));
        }
    }

    #endregion
}