namespace Base.Interfaces
{
    public interface IStateButton
    {
        #region Properties

        bool IsEnabled { get; set; }
        int CheckStates { get; set; }
        int CurrentState { get; set; }

        #endregion
    }
}