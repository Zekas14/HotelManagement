using FluentValidation;
using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManagement.Features.RoomManagement.Rooms
{

    #region Command 
    public record AddRoomCommand(int RoomNumber, string Name ,string ImageUrl,int Capacity, string Type, decimal PricePerNight, bool IsAvailable) : IRequest<RequestResult<bool>>;
    #endregion

    #region DTO & Validator

    public class AddRoomDto
    {

        public int RoomNumber { get; set; }

        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public int Capacity { get; set; }
        public string? Type { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
    }
    public class AddRoomCommandValidator : AbstractValidator<AddRoomCommand>
    {
        private readonly IGenericRepository<Room> repository;
        public AddRoomCommandValidator(IGenericRepository<Room> repository)
        {
            this.repository = repository;
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
            return !repository.GetAll().Any(r => r.RoomNumber == roomNumber);
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
    public class AddRoomEndpoint(IMediator mediator , IValidator<AddRoomCommand> validator) : PostEndpoint<AddRoomCommand, bool>(validator , mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/add";

        public override async Task HandleAsync(AddRoomCommand req, CancellationToken ct)
        {
            var validationResult = await Validate(req);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);
                return;
            }
            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    
    }
    #endregion
}

