namespace ProcessGraph.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; private set; }
}

