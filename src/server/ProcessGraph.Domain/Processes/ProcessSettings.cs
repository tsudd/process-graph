namespace ProcessGraph.Domain.Processes;

public sealed record ProcessSettings(UnitOfMeasure Unit)
{
    public static ProcessSettings CreateDefault()
    {
        return new ProcessSettings(UnitOfMeasure.Days);
    }
}

