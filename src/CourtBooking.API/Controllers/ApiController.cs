using CourtBooking.Domain.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CourtBooking.API.Controllers;

// Todos los controllers heredan de este
// Centraliza la lógica de conversión Result → HTTP response
[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;

    protected ApiController(ISender sender)
    {
        Sender = sender;
    }

    // Convierte Result<T> en IActionResult
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new
        {
            code = result.Error.Code,
            message = result.Error.Message
        });
    }

    // Para resultados sin valor de retorno
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        return BadRequest(new
        {
            code = result.Error.Code,
            message = result.Error.Message
        });
    }

    // Para recursos creados — devuelve 201 Created
    protected IActionResult HandleCreatedResult<T>(Result<T> result, string routeName, object routeValues)
    {
        if (result.IsSuccess)
            return CreatedAtRoute(routeName, routeValues, result.Value);

        return BadRequest(new
        {
            code = result.Error.Code,
            message = result.Error.Message
        });
    }

    // Helper para obtener el UserId del JWT claim
    protected string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? string.Empty;
    }
}