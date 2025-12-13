using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Domain.Processes;

public record ProcessTotals(int Total, UnitOfMeasure Unit, string[] LongestPath, bool Valid)
{
    public static ProcessTotals GetDefault(UnitOfMeasure unit)
    {
        return new ProcessTotals(0, unit, [], false);
    }
}

