using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using HotelManagement.Features.RoomManagement.Facilities;
using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Repositories;

namespace HotelManagement.Tests.Facilities
{
    public class AddFacillityCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldAddFacilityAndReturnSuccess()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<Facility>>();
            Facility? capturedFacility = null;
            repoMock.Setup(r => r.Add(It.IsAny<Facility>()))
                .Callback<Facility>(f => capturedFacility = f);

            var handler = new AddFacillityCommandHandler(repoMock.Object);
            var command = new AddFacillityCommand("Pool");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Facility Added Successfully", result.Message);
            repoMock.Verify(r => r.Add(It.IsAny<Facility>()), Times.Once);
            Assert.NotNull(capturedFacility);
            Assert.Equal("Pool", capturedFacility!.Name);
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenRepositoryThrows()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<Facility>>();
            repoMock.Setup(r => r.Add(It.IsAny<Facility>()))
                .Throws(new InvalidOperationException("repository failure"));

            var handler = new AddFacillityCommandHandler(repoMock.Object);
            var command = new AddFacillityCommand("Gym");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}