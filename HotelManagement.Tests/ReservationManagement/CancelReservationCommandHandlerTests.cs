using Moq;
using MediatR;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using HotelManagement.Features.ReservationManagement.Reservations.CancelReservation.Commands;
using HotelManagement.Features.ReservationManagement.Reservations.CancelReservation.Queries;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;

namespace HotelManagement.Tests.ReservationManagement
{
    public class CancelReservationCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WhenReservationNotAllowed_ReturnsFailureAndDoesNotCallXADeleteOrSave()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<Reservation>>();
            var mediatorMock = new Mock<IMediator>();

            var failureResult = RequestResult<bool>.Failure(ErrorCode.BadRequest, "cannot cancel");
            mediatorMock.Setup(m => m.Send(It.IsAny<IsReservationAllowedTobeCancelledQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(failureResult);

            var handler = new CancelReservationCommandHandler(repoMock.Object, mediatorMock.Object);
            var command = new CancelReservationCommand(42, "notes");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(failureResult.Message, result.Message);
            Assert.Equal(failureResult.ErrorCode, result.ErrorCode);
            repoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
            repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenReservationAllowed_DeletesReservationAndSavesChanges_ReturnsSuccess()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<Reservation>>();
            var mediatorMock = new Mock<IMediator>();

            var allowedResult = RequestResult<bool>.Success(true, "Reservation can be cancelled");
            mediatorMock.Setup(m => m.Send(It.IsAny<IsReservationAllowedTobeCancelledQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(allowedResult);

            int? deletedId = null;
            repoMock.Setup(r => r.Delete(It.IsAny<int>()))
                    .Callback<int>(id => deletedId = id);
            repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

            var handler = new CancelReservationCommandHandler(repoMock.Object, mediatorMock.Object);
            var command = new CancelReservationCommand(5, "some notes");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Reservation cancelled successfully", result.Message);
            repoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Once);
            Assert.Equal(5, deletedId);
            repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenSaveChangesThrows_PropagatesException()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<Reservation>>();
            var mediatorMock = new Mock<IMediator>();

            var allowedResult = RequestResult<bool>.Success(true, "Reservation can be cancelled");
            mediatorMock.Setup(m => m.Send(It.IsAny<IsReservationAllowedTobeCancelledQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(allowedResult);

            repoMock.Setup(r => r.Delete(It.IsAny<int>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("db fail"));

            var handler = new CancelReservationCommandHandler(repoMock.Object, mediatorMock.Object);
            var command = new CancelReservationCommand(7, "notes");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            repoMock.Verify(r => r.Delete(7), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
    