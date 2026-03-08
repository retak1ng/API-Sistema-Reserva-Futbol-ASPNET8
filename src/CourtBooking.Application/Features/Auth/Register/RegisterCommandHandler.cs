using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.Abstractions.Services;
using CourtBooking.Application.DTOs.Auth;
using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Errors;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verificar que el email no esté en uso
        var emailExists = await _userRepository.ExistsByEmailAsync(
            request.Email,
            cancellationToken);

        if (emailExists)
            return Result.Failure<AuthResponse>(DomainErrors.User.EmailAlreadyInUse);

        // 2. Hashear la contraseña — NUNCA guardar en texto plano
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Crear el usuario (Factory Method valida las reglas de dominio)
        var userResult = User.Create(
            name: request.Name,
            email: request.Email,
            passwordHash: passwordHash);

        if (userResult.IsFailure)
            return Result.Failure<AuthResponse>(userResult.Error);

        var user = userResult.Value;

        // 4. Persistir
        await _userRepository.AddAsync(user, cancellationToken);

        // 5. Generar JWT
        var token = _jwtTokenGenerator.GenerateToken(user);

        // 6. Retornar respuesta
        return Result.Success(new AuthResponse(
            UserId: user.Id,
            Name: user.Name,
            Email: user.Email,
            Role: user.Role.ToString(),
            Token: token));
    }
}