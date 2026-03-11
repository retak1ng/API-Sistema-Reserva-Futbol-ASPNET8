using FluentValidation;

namespace CourtBooking.Application.Features.Reservations.Create;

public class CreateReservationCommandValidator
    : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El Id del usuario es requerido.");

        RuleFor(x => x.AvailabilityId)
            .NotEmpty().WithMessage("El Id de disponibilidad es requerido.");
    }
}