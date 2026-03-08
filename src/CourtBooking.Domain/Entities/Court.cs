using CourtBooking.Domain.Errors;
using CourtBooking.Domain.Primitives;

namespace CourtBooking.Domain.Entities;

public sealed class Court : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal PricePerHour { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Court(string id, string name, string description, decimal pricePerHour)
        : base(id)
    {
        Name = name;
        Description = description;
        PricePerHour = pricePerHour;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    private Court() { }

    public static Result<Court> Create(string name, string description, decimal pricePerHour)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Court>(DomainErrors.Court.EmptyName);

        if (pricePerHour <= 0)
            return Result.Failure<Court>(DomainErrors.Court.InvalidPrice);

        var court = new Court(
            id: Guid.NewGuid().ToString(),
            name: name.Trim(),
            description: description.Trim(),
            pricePerHour: pricePerHour
        );

        return Result.Success(court);
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}