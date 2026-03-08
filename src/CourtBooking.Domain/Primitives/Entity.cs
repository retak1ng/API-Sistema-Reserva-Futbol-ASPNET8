namespace CourtBooking.Domain.Primitives;

public abstract class Entity
{
    protected Entity(string id)
    {
        Id = id;
    }

    // Constructor vacío requerido por Firestore para deserialización
    protected Entity() { }

    public string Id { get; private set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id == other.Id;
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);

    public override int GetHashCode() => Id.GetHashCode();
}