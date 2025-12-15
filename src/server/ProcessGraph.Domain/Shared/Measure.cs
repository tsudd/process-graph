namespace ProcessGraph.Domain.Shared;

public sealed record Measure(int Value, UnitOfMeasure Unit)
{
    public static Measure operator +(Measure first, Measure second)
    {
        if (first.Unit != second.Unit)
        {
            throw new InvalidOperationException("Unit of measure should be equal");
        }

        return first with { Value = first.Value + second.Value };
    }
}