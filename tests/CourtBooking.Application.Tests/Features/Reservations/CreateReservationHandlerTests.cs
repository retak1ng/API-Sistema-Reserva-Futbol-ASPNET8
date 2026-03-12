using CourtBooking.Application.Features.Reservations.Create;
using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.Tests.Helpers;
using CourtBooking.Domain.Errors;
using FluentAssertions;
using Moq;

namespace CourtBooking.Application.Tests.Features.Reservations;

public class CreateReservationHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IAvailabilityRepository> _availabilityRepositoryMock;
    private readonly Mock<ICourtRepository> _courtRepositoryMock;
    private readonly CreateReservationCommandHandler _handler;

    public CreateReservationHandlerTests()
    {
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _availabilityRepositoryMock = new Mock<IAvailabilityRepository>();
        _courtRepositoryMock = new Mock<ICourtRepository>();

        _handler = new CreateReservationCommandHandler(
            _reservationRepositoryMock.Object,
            _availabilityRepositoryMock.Object,
            _courtRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccessWithReservation()
    {
        // Arrange
        var court = MockHelpers.CreateValidCourt(pricePerHour: 100m);
        var availability = MockHelpers.CreateValidAvailability(court.Id);
        var userId = Guid.NewGuid().ToString();

        var command = new CreateReservationCommand(
            UserId: userId,
            AvailabilityId: availability.Id);

        _availabilityRepositoryMock
            .Setup(x => x.GetByIdAsync(availability.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(availability);

        _courtRepositoryMock
            .Setup(x => x.GetByIdAsync(court.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(court);

        _availabilityRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Availability>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _reservationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Reservation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
        result.Value.TotalPrice.Should().Be(100m); // 1 hora × $100

        // Verificar que se persistieron ambos documentos
        _availabilityRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Availability>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _reservationRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Domain.Entities.Reservation>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithAlreadyBookedSlot_ReturnsFailureResult()
    {
        // Arrange
        var court = MockHelpers.CreateValidCourt();
        var availability = MockHelpers.CreateValidAvailability(court.Id);
        availability.Book(); // Simular slot ya reservado

        var command = new CreateReservationCommand(
            UserId: Guid.NewGuid().ToString(),
            AvailabilityId: availability.Id);

        _availabilityRepositoryMock
            .Setup(x => x.GetByIdAsync(availability.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(availability);

        _courtRepositoryMock
            .Setup(x => x.GetByIdAsync(court.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(court);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Availability.AlreadyBooked);

        // Verificar que NUNCA se guardó la reserva
        _reservationRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Domain.Entities.Reservation>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistentAvailability_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateReservationCommand(
            UserId: Guid.NewGuid().ToString(),
            AvailabilityId: "id-inexistente");

        _availabilityRepositoryMock
            .Setup(x => x.GetByIdAsync("id-inexistente", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Availability?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Availability.NotFound);
    }
}