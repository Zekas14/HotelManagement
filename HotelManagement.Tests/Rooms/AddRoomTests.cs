using Xunit;
using Moq;
using HotelManagement.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Enums;
using HotelManagement.Features.RoomManagement.Rooms.Commands;

namespace HotelManagement.Tests.Rooms
{
    public class AddRoomTests
    {
        private readonly Mock<IGenericRepository<Room>> _repositoryMock;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly AddRoomCommandHandler _handler;

        public AddRoomTests()
        {
            _repositoryMock = new Mock<IGenericRepository<Room>>();
            _cacheMock = new Mock<IMemoryCache>();
            _handler = new AddRoomCommandHandler(_repositoryMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAddRoomAndReturnSuccess()
        {
            
            // Arrange
            var command = new AddRoomCommand(101, "Deluxe Room", "url", 2, "Deluxe", 500, true);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Room added successfully.", result.Message);
            _repositoryMock.Verify(r => r.Add(It.IsAny<Room>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _cacheMock.Verify(c => c.Remove("rooms"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenInvalidRoomTypeProvided()
        {
            // Arrange
            var command = new AddRoomCommand(103, "Invalid Type", "url", 2, "InvalidType", 300, true);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldRemoveCacheAfterSuccessfulAdd()
        {
            // Arrange
            var command = new AddRoomCommand(104, "CacheTest Room", "url", 3, "Single", 200, true);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _cacheMock.Verify(c => c.Remove("rooms"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectRoomObjectToRepository()
        {
            // Arrange
            var command = new AddRoomCommand(105, "Check Mapping", "url", 2, "Double", 150, true);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            Room? capturedRoom = null;
            _repositoryMock.Setup(r => r.Add(It.IsAny<Room>()))
                .Callback<Room>(room => capturedRoom = room);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedRoom);
            Assert.Equal(command.RoomNumber, capturedRoom.RoomNumber);
            Assert.Equal(RoomType.Double, capturedRoom.Type);
        }

        [Fact]
        public async Task Handle_ShouldRespectCancellationToken()
        {
            // Arrange
            var command = new AddRoomCommand(106, "CancelTest", "url", 1, "Single", 100, true);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                Task.Run(() => _handler.Handle(command, cts.Token), cts.Token));
        }
    }
}
