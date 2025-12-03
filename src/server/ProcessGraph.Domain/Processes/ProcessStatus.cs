namespace ProcessGraph.Domain;

public enum ProcessStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Locked = 4,
}
