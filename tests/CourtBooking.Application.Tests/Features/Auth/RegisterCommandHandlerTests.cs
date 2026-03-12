using CourtBooking.Application.Features.Auth.Register;
using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.Tests.Helpers;
using CourtBooking.Domain.Errors;
using FluentAssertions;
using Moq;

namespace CourtBooking.Application.Tests.Features.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        var jwtGeneratorMock = MockHelpers.CreateJwtGeneratorMock();

        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            jwtGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccessWithToken()
    {
        // Arrange
        var command = new RegisterCommand(
            Name: "Martin Rojas",
            Email: "martin@email.com",
            Password: "Password1");

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.Email.Should().Be(command.Email);

        // Verificar que AddAsync fue llamado exactamente una vez
        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ReturnsFailureResult()
    {
        // Arrange
        var command = new RegisterCommand(
            Name: "Martin Rojas",
            Email: "existing@email.com",
            Password: "Password1");

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Email ya existe

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmailAlreadyInUse);

        // Verificar que NUNCA se llamó AddAsync
        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}