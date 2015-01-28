namespace Base.Interfaces
{
    public interface IStateButton
    {
        bool IsEnabled { get; set; }
        int CheckStates { get; set; }
        int CurrentState { get; set; }
    }
}
