using FluentValidation;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Data;
using HotelManagement.Data.Repositories;
using HotelManagement.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Features.RoomManagement.Rooms
{
    #region Command 
    public record AddRoomCommand(int RoomNumber, string Name ,string ImageUrl,int Capacity, string Type, decimal PricePerNight, bool IsAvailable) : IRequest<RequestResult<bool>>;
    #endregion

    #region DTO & Validator

    public class AddRoomDto
    {

        public int RoomNumber { get; set; }

        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Capacity { get; set; }
        [AllowedValues([ "Single", "Double", "Suite", "Deluxe" ])]
        public string Type { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
    }
    public class AddRoomCommandValidator : AbstractValidator<AddRoomDto>
    {
        private readonly ApplicationDbContext context;

        public AddRoomCommandValidator(ApplicationDbContext context)
        {
            this.context = context;
            RuleFor(x => x.RoomNumber).GreaterThan(0);
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(20)
                .Must(type => new List<string> { "Single", "Double", "Suite", "Deluxe" }.Contains(type))
                .WithMessage("Type must be one of: Single, Double, Suite, Deluxe.");
            RuleFor(x => x.PricePerNight).GreaterThan(0);
            RuleFor(x=>x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
            RuleFor(x=>x.ImageUrl).NotEmpty().MinimumLength(2);
            RuleFor(x => x.RoomNumber)
                .Must(BeUniqueRoomNumber)
                .WithMessage("Room number must be unique.");
        }
        private bool BeUniqueRoomNumber(int roomNumber)
        {
            return !context.Rooms.Any(r => r.RoomNumber == roomNumber);
        }
    }
    #endregion

    #region Command Handler

    public class AddRoomCommandHandler(IGenericRepository<Room> repository,IMemoryCache cache) : IRequestHandler<AddRoomCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Room> repository = repository;
        private readonly IMemoryCache cache = cache;

        public async Task<RequestResult<bool>> Handle(AddRoomCommand request, CancellationToken cancellationToken)
        {
            
            repository.Add(new Room
            {
                RoomNumber = request.RoomNumber,
                Name = request.Name,
                ImageUrl = request.ImageUrl,
                Capacity = request.Capacity,
                Type = Enum.Parse<RoomType>(request.Type),
                PricePerNight = request.PricePerNight,
                IsAvailable = request.IsAvailable,

            });
            await repository.SaveChangesAsync();
            cache.Remove("rooms");
            return RequestResult<bool>.Success(true, "Room added successfully.");
        }
    }
    #endregion

    #region Endpoint 
    public class AddRoomEndpoint : PostModule<AddRoomDto>
    {
        protected override string Route => "api/rooms/add";

        protected override Func<AddRoomDto, IMediator, Task<IResult>> Handler =>
            async (AddRoomDto dto, IMediator mediator) =>
            {
                var command = new AddRoomCommand(dto.RoomNumber,dto.Name,dto.ImageUrl, dto.Capacity, dto.Type, dto.PricePerNight, dto.IsAvailable);
                var result = await mediator.Send(command);

                return result.IsSuccess
                    ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                    : FailureEndpointResult<bool>.BadRequest(result.Message);
            };
    }
    #endregion
}

