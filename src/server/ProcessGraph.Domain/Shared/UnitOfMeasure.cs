namespace ProcessGraph.Domain.Shared;

public record UnitOfMeasure
{
    public static readonly UnitOfMeasure Points = new("points");
    public static readonly UnitOfMeasure Days = new("days");
    public static readonly UnitOfMeasure Weeks = new("weeks");

    private UnitOfMeasure(string unit) => Unit = unit;

    public string Unit { get; init; }

    public static UnitOfMeasure FromName(string name)
    {
        return All.FirstOrDefault(u => u.Unit == name)
            ?? throw new ApplicationException("Unit of measure name is invalid");
    }

    public static readonly IReadOnlyCollection<UnitOfMeasure> All = [Points, Days, Weeks];
    public override string ToString()
    {
        return Unit;
    }
}

