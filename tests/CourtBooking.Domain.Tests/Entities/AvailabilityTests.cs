using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Errors;
using FluentAssertions;

namespace CourtBooking.Domain.Tests.Entities;

public class AvailabilityTests
{
    // ── Datos válidos para reutilizar ──
    private static readonly string ValidCourtId = Guid.NewGuid().ToString();
    private static readonly DateOnly ValidDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
    private static readonly TimeOnly ValidStartTime = new(10, 0);
    private static readonly TimeOnly ValidEndTime = new(11, 0);

    #region Create

    [Fact]
    public void Create_WithValidData_ReturnsSuccessResult()
    {
        // Arrange & Act
        var result = Availability.Create(
            courtId: ValidCourtId,
            date: ValidDate,
            startTime: ValidStartTime,
            endTime: ValidEndTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IsBooked.Should().BeFalse();
        result.Value.CourtId.Should().Be(ValidCourtId);
    }

    [Fact]
    public void Create_WithPastDate_ReturnsFailureResult()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        // Act
        var result = Availability.Create(
            courtId: ValidCourtId,
            date: pastDate,
            startTime: ValidStartTime,
            endTime: ValidEndTime);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Availability.PastDate);
    }

    [Fact]
    public void Create_WhenStartTimeIsAfterEndTime_ReturnsFailureResult()
    {
        // Arrange
        var startTime = new TimeOnly(11, 0);
        var endTime = new TimeOnly(10, 0);

        // Act
        var result = Availability.Create(
            courtId: ValidCourtId,
            date: ValidDate,
            startTime: startTime,
            endTime: endTime);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Availability.InvalidTimeRange);
    }

    [Fact]
    public void Create_WithEmptyCourtId_ReturnsFailureResult()
    {
        // Act
        var result = Availability.Create(
            courtId: string.Empty,
            date: ValidDate,
            startTime: ValidStartTime,
            endTime: ValidEndTime);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Availability.EmptyCourtId);
    }

    #endregion

    #region Book

    [Fact]
    public void Book_WhenNotBooked_ReturnsSuccessAndSetsIsBookedTrue()
    {
        // Arrange
        var availability = Availability.Create(
            ValidCourtId, ValidDate, ValidStartTime, ValidEndTime).Value;

        // Act
        var result = availability.Book();

        // Assert
        result.IsSuccess.Should().BeTrue();
        availability.IsBooked.Should().BeTrue();
    }

    [Fact]
    public void Book_WhenAlreadyBooked_ReturnsFailureResult()
    {
        // Arrange
        var availability = Availability.Create(
            ValidCourtId, ValidDate, ValidStartTime, ValidEndTime).Value;

        availability.Book(); // Primera reserva

        // Act
        var result = availability.Book(); // Segunda reserva — debe fallar

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Availability.AlreadyBooked);
    }

    #endregion

    #region Release

    [Fact]
    public void Release_WhenBooked_ReturnsSuccessAndSetsIsBookedFalse()
    {
        // Arrange
        var availability = Availability.Create(
            ValidCourtId, ValidDate, ValidStartTime, ValidEndTime).Value;
        availability.Book();

        // Act
        var result = availability.Release();

        // Assert
        result.IsSuccess.Should().BeTrue();
        availability.IsBooked.Should().BeFalse();
    }

    [Fact]
    public void Release_WhenNotBooked_ReturnsFailureResult()
    {
        // Arrange
        var availability = Availability.Create(
            ValidCourtId, ValidDate, ValidStartTime, ValidEndTime).Value;

        // Act
        var result = availability.Release();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Availability.NotBooked);
    }

    #endregion
}