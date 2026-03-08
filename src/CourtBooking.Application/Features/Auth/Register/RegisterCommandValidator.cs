using FluentValidation;

namespace CourtBooking.Application.Features.Auth.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido.")
            .EmailAddress().WithMessage("El formato del email no es válido.")
            .MaximumLength(200).WithMessage("El email no puede superar los 200 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe tener al menos una mayúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe tener al menos un número.");
    }
}