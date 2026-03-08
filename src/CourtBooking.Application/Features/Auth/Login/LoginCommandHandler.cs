using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.Abstractions.Services;
using CourtBooking.Application.DTOs.Auth;
using CourtBooking.Domain.Errors;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Buscar usuario por email
        var user = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        // Mismo error para usuario no encontrado y contraseña incorrecta
        // Nunca revelar cuál de los dos falló → seguridad
        if (user is null)
            return Result.Failure<AuthResponse>(DomainErrors.User.InvalidCredentials);

        // 2. Verificar contraseña
        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
            return Result.Failure<AuthResponse>(DomainErrors.User.InvalidCredentials);

        // 3. Generar token
        var token = _jwtTokenGenerator.GenerateToken(user);

        return Result.Success(new AuthResponse(
            UserId: user.Id,
            Name: user.Name,
            Email: user.Email,
            Role: user.Role.ToString(),
            Token: token));
    }
}