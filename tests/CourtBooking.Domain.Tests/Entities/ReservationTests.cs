using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Enums;
using CourtBooking.Domain.Errors;
using FluentAssertions;

namespace CourtBooking.Domain.Tests.Entities;

public class ReservationTests
{
    private static readonly string ValidUserId = Guid.NewGuid().ToString();
    private static readonly string ValidCourtId = Guid.NewGuid().ToString();
    private static readonly string ValidAvailabilityId = Guid.NewGuid().ToString();
    private static readonly DateOnly ValidDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
    private static readonly TimeOnly ValidStartTime = new(10, 0);
    private static readonly TimeOnly ValidEndTime = new(11, 0);
    private const decimal ValidPrice = 100m;

    private static Reservation CreateValidReservation() =>
        Reservation.Create(
            userId: ValidUserId,
            courtId: ValidCourtId,
            availabilityId: ValidAvailabilityId,
            date: ValidDate,
            startTime: ValidStartTime,
            endTime: ValidEndTime,
            totalPrice: ValidPrice).Value;

    #region Create

    [Fact]
    public void Create_WithValidData_ReturnsSuccessWithConfirmedStatus()
    {
        // Act
        var result = Reservation.Create(
            userId: ValidUserId,
            courtId: ValidCourtId,
            availabilityId: ValidAvailabilityId,
            date: ValidDate,
            startTime: ValidStartTime,
            endTime: ValidEndTime,
            totalPrice: ValidPrice);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(ReservationStatus.Confirmed);
        result.Value.TotalPrice.Should().Be(ValidPrice);
    }

    [Fact]
    public void Create_WithNegativePrice_ReturnsFailureResult()
    {
        // Act
        var result = Reservation.Create(
            userId: ValidUserId,
            courtId: ValidCourtId,
            availabilityId: ValidAvailabilityId,
            date: ValidDate,
            startTime: ValidStartTime,
            endTime: ValidEndTime,
            totalPrice: -1m);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Reservation.InvalidPrice);
    }

    #endregion

    #region Cancel

    [Fact]
    public void Cancel_WhenConfirmed_ReturnsSuccessAndSetsCancelledStatus()
    {
        // Arrange
        var reservation = CreateValidReservation();

        // Act
        var result = reservation.Cancel();

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ReturnsFailureResult()
    {
        // Arrange
        var reservation = CreateValidReservation();
        reservation.Cancel(); // Primera cancelación

        // Act
        var result = reservation.Cancel(); // Segunda cancelación — debe fallar

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Reservation.AlreadyCancelled);
    }

    #endregion
}