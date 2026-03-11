using FluentValidation;

namespace CourtBooking.Application.Features.Courts.Create;

public class CreateCourtCommandValidator : AbstractValidator<CreateCourtCommand>
{
    public CreateCourtCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede superar 500 caracteres.");

        RuleFor(x => x.PricePerHour)
            .GreaterThan(0).WithMessage("El precio por hora debe ser mayor a cero.");
    }
}