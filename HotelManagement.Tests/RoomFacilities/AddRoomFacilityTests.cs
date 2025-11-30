/*
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Features.RoomManagement.RoomFacilities;
using HotelManagement.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace HotelManagement.Tests.Features.RoomManagement.RoomFacilities
{
    public class AddRoomFacilityTests
    {
        [Fact]
        public async Task Handler_Should_Return_Failure_If_Facility_Already_Assigned()
        {
            // Arrange
            var existing = new RoomFacility { Id = 1, RoomId = 10, FacilityId = 20 };

            var repoMock = new Mock<IGenericRepository<RoomFacility>>();
            repoMock.Setup(r => r.GetAll()).Returns(new List<RoomFacility> { existing }.AsQueryable());
            // Ensure Add/SaveChanges not called
            repoMock.Setup(r => r.Add(It.IsAny<RoomFacility>())).Verifiable();
            repoMock.Setup(r => r.SaveChanges()).Verifiable();

            var cacheMock = new Mock<IMemoryCache>();
            cacheMock.Setup(c => c.Remove(It.IsAny<object>())).Verifiable();

            var handler = new AddRoomFacilityCommandHandler(repoMock.Object, cacheMock.Object);

            // Act
            var result = await handler.Handle(new AddRoomFacilityCommand(existing.RoomId, existing.FacilityId), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Facility already assigned to room.", result.Message);

            // verify no add/save/cache-remove occurred
            repoMock.Verify(r => r.Add(It.IsAny<RoomFacility>()), Times.Never);
            repoMock.Verify(r => r.SaveChanges(), Times.Never);
            cacheMock.Verify(c => c.Remove(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task Handler_Should_Add_RoomFacility_And_Save_And_Remove_Cache_When_Not_Assigned()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<RoomFacility>>();
            repoMock.Setup(r => r.GetAll()).Returns(Enumerable.Empty<RoomFacility>().AsQueryable());
            repoMock.Setup(r => r.Add(It.IsAny<RoomFacility>())).Verifiable();
            repoMock.Setup(r => r.SaveChanges()).Verifiable();

            var cacheMock = new Mock<IMemoryCache>();
            cacheMock.Setup(c => c.Remove("rooms")).Verifiable();

            var handler = new AddRoomFacilityCommandHandler(repoMock.Object, cacheMock.Object);

            var cmd = new AddRoomFacilityCommand(RoomId: 7, FacilityId: 33);

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Facility assigned to room successfully", result.Message);

            repoMock.Verify(r => r.Add(It.Is<RoomFacility>(rf => rf.RoomId == cmd.RoomId && rf.FacilityId == cmd.FacilityId)), Times.Once);
            repoMock.Verify(r => r.SaveChanges(), Times.Once);
            cacheMock.Verify(c => c.Remove("rooms"), Times.Once);
        }
    }
}*/