using CourtBooking.Application.DTOs.Court;
using CourtBooking.Application.Features.Courts.Create;
using CourtBooking.Application.Features.Courts.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtBooking.API.Controllers;

/// <summary>
/// Gestión de canchas deportivas.
/// </summary>
public class CourtsController : ApiController
{
    public CourtsController(ISender sender) : base(sender) { }

    /// <summary>
    /// Obtiene todas las canchas activas. (Público)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CourtResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAllCourtsQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crea una nueva cancha. (Solo Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CourtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourtRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCourtCommand(
            request.Name,
            request.Description,
            request.PricePerHour);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}