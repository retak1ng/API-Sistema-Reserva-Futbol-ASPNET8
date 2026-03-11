using CourtBooking.Application.DTOs.Auth;
using CourtBooking.Application.Features.Auth.Login;
using CourtBooking.Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtBooking.API.Controllers;

/// <summary>
/// Endpoints de autenticación: registro y login.
/// </summary>
public class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender) { }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Name,
            request.Email,
            request.Password);

        var result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Autentica un usuario y devuelve un JWT.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}