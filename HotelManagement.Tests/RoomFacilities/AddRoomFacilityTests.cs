using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using HotelManagement.Data;
using HotelManagement.Data.Repositories;
using HotelManagement.Features.RoomManagement.RoomFacilities;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace HotelManagement.Tests.Features.RoomManagement.RoomFacilities
{
    public class AddRoomFacilityTests
    {
        // Validator tests

        [Fact]
        public void Validator_Should_Fail_When_RoomId_Is_Not_Positive()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .Options;
            using var context = new ApplicationDbContext(options);

            var validator = new AddRoomFacilityCommandValidator(context);

            var result = validator.Validate(new AddRoomFacilityCommand(0, 1));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(AddRoomFacilityCommand.RoomId));
        }

        [Fact]
        public void Validator_Should_Fail_When_FacilityId_Is_Not_Positive()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .Options;
            using var context = new ApplicationDbContext(options);

            var validator = new AddRoomFacilityCommandValidator(context);

            var result = validator.Validate(new AddRoomFacilityCommand(1, 0));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(AddRoomFacilityCommand.FacilityId));
        }

        [Fact]
        public void Validator_Should_Fail_When_Room_Does_Not_Exist()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .Options;
            using var context = new ApplicationDbContext(options);

            // no rooms seeded -> should fail the RoomExists rule
            var validator = new AddRoomFacilityCommandValidator(context);

            var result = validator.Validate(new AddRoomFacilityCommand(999, 1));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Room does not exist"));
        }

        [Fact]
        public void Validator_Should_Pass_When_Room_Exists_And_Ids_Are_Positive()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .Options;
            using var context = new ApplicationDbContext(options);

            context.Rooms.Add(new Room { Id = 5, RoomNumber = 101 });
            context.SaveChanges();

            var validator = new AddRoomFacilityCommandValidator(context);

            var result = validator.Validate(new AddRoomFacilityCommand(5, 1));

            Assert.True(result.IsValid);
        }

        // Handler tests

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
}