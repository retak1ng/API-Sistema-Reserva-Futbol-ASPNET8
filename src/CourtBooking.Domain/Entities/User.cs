using CourtBooking.Domain.Enums;
using CourtBooking.Domain.Errors;
using CourtBooking.Domain.Primitives;

namespace CourtBooking.Domain.Entities;

public sealed class User : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Constructor para crear un nuevo usuario
    private User(string id, string name, string email, string passwordHash, UserRole role)
        : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    // Constructor vacío para Firestore
    private User() { }

    // Factory method — única forma de crear un User válido
    public static Result<User> Create(
        string name,
        string email,
        string passwordHash,
        UserRole role = UserRole.User)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<User>(DomainErrors.User.EmptyName);

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<User>(DomainErrors.User.EmptyEmail);

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure<User>(DomainErrors.User.EmptyPassword);

        var user = new User(
            id: Guid.NewGuid().ToString(),
            name: name.Trim(),
            email: email.Trim().ToLowerInvariant(),
            passwordHash: passwordHash,
            role: role
        );

        return Result.Success(user);
    }
}