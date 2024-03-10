public struct ObjectiveCompletedEvent
{
    public Objective CompletedObjective { get; }

    public ObjectiveCompletedEvent(Objective _completedObjective)
    {
        CompletedObjective = _completedObjective;
    }
}
