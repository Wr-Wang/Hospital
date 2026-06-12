namespace Hospital.Domain;

public abstract class Entity
{
    public long Id { get; protected set; }

    protected Entity() { }

    protected Entity(long id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity left, Entity right) => Equals(left, right);

    public static bool operator !=(Entity left, Entity right) => !Equals(left, right);
}