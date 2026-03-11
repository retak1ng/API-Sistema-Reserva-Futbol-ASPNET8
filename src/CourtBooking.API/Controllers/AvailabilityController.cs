using CourtBooking.Application.DTOs.Availability;
using CourtBooking.Application.Features.Availability.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtBooking.API.Controllers;

/// <summary>
/// Gestión de disponibilidad de canchas.
/// </summary>
public class AvailabilityController : ApiController
{
    public AvailabilityController(ISender sender) : base(sender) { }

    /// <summary>
    /// Crea un slot de disponibilidad para una cancha. (Solo Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAvailabilityCommand(
            request.CourtId,
            request.Date,
            request.StartTime,
            request.EndTime);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}