using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Domain.Processes;

public record ProcessTotals(int Total, UnitOfMeasure Unit, string[] LongestPath, bool Valid);

