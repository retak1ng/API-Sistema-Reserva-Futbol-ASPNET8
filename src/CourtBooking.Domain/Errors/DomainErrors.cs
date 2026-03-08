using CourtBooking.Domain.Primitives;

namespace CourtBooking.Domain.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error EmptyName =
            new("User.EmptyName", "El nombre no puede estar vacío.");

        public static readonly Error EmptyEmail =
            new("User.EmptyEmail", "El email no puede estar vacío.");

        public static readonly Error EmptyPassword =
            new("User.EmptyPassword", "La contraseña no puede estar vacía.");

        public static readonly Error EmailAlreadyInUse =
            new("User.EmailAlreadyInUse", "El email ya está registrado.");

        public static readonly Error NotFound =
            new("User.NotFound", "El usuario no fue encontrado.");

        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "Las credenciales son incorrectas.");
    }

    public static class Court
    {
        public static readonly Error EmptyName =
            new("Court.EmptyName", "El nombre de la cancha no puede estar vacío.");

        public static readonly Error InvalidPrice =
            new("Court.InvalidPrice", "El precio por hora debe ser mayor a cero.");

        public static readonly Error NotFound =
            new("Court.NotFound", "La cancha no fue encontrada.");

        public static readonly Error NotActive =
            new("Court.NotActive", "La cancha no está disponible.");
    }

    public static class Availability
    {
        public static readonly Error EmptyCourtId =
            new("Availability.EmptyCourtId", "El Id de la cancha es requerido.");

        public static readonly Error InvalidTimeRange =
            new("Availability.InvalidTimeRange", "La hora de inicio debe ser menor a la hora de fin.");

        public static readonly Error PastDate =
            new("Availability.PastDate", "No se puede crear disponibilidad en fechas pasadas.");

        public static readonly Error AlreadyBooked =
            new("Availability.AlreadyBooked", "Este turno ya fue reservado.");

        public static readonly Error NotBooked =
            new("Availability.NotBooked", "Este turno no está reservado.");

        public static readonly Error NotFound =
            new("Availability.NotFound", "La disponibilidad no fue encontrada.");
    }

    public static class Reservation
    {
        public static readonly Error InvalidPrice =
            new("Reservation.InvalidPrice", "El precio total no puede ser negativo.");

        public static readonly Error AlreadyCancelled =
            new("Reservation.AlreadyCancelled", "La reserva ya fue cancelada.");

        public static readonly Error NotFound =
            new("Reservation.NotFound", "La reserva no fue encontrada.");

        public static readonly Error Unauthorized =
            new("Reservation.Unauthorized", "No tenés permisos para modificar esta reserva.");
    }
}