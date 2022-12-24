public class QuestTaskProgressModel
{
    public string QuestId;
    public string QuestTaskId;

    // for dialogue completions
    public bool DialogueCompleted;

    // for dispatch completions
    public int DispatchCount = 0;

    // for item completions
    public bool ItemsTurnedIn;

    // de facto status of completion
    public bool IsComplete;
}