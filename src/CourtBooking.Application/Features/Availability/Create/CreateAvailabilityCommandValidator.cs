using FluentValidation;

namespace CourtBooking.Application.Features.Availability.Create;

public class CreateAvailabilityCommandValidator
    : AbstractValidator<CreateAvailabilityCommand>
{
    public CreateAvailabilityCommandValidator()
    {
        RuleFor(x => x.CourtId)
            .NotEmpty().WithMessage("El Id de la cancha es requerido.");

        RuleFor(x => x.Date)
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("La fecha no puede ser en el pasado.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("La hora de inicio debe ser menor a la hora de fin.");
    }
}