using CourtBooking.Application.DTOs.Reservation;
using CourtBooking.Application.Features.Reservations.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtBooking.API.Controllers;

/// <summary>
/// Gestión de reservas de canchas.
/// </summary>
[Authorize]
public class ReservationsController : ApiController
{
    public ReservationsController(ISender sender) : base(sender) { }

    /// <summary>
    /// Crea una nueva reserva para el usuario autenticado.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var command = new CreateReservationCommand(
            UserId: userId,
            AvailabilityId: request.AvailabilityId);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}