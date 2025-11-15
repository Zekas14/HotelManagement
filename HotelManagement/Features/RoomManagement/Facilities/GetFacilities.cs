using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Facilities
{
    #region Query
    public record GetFacilitiesQuery : IRequest<RequestResult<IReadOnlyList<GetFacilitiesResponseDto>>>;
    #endregion

    #region Dto
    public record GetFacilitiesResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
    }
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
                return Task.FromResult(RequestResult<IReadOnlyList<GetFacilitiesResponseDto>>.Failure("No facilities found"));
            }

            return Task.FromResult(RequestResult<IReadOnlyList<GetFacilitiesResponseDto>>.Success(data.ToList(), "Facilities retrieved successfully"));
        }
    }
    #endregion

    #region Endpoint
    public class GetFacilitiesEndPoint(IMediator mediator) : GetEndpoint<GetFacilitiesQuery>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/facilities";

        public override async Task HandleAsync(CancellationToken ct)
        {
            var query = new GetFacilitiesQuery();
            var result = await mediator.Send(query, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<IReadOnlyList<GetFacilitiesResponseDto>>(result.Data, result.Message)
                : FailureEndpointResult<IReadOnlyList<GetFacilitiesResponseDto>>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }
    #endregion
}
