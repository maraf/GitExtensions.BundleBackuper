namespace Neptuo.StateMachines
{
    /// <summary>
    /// Represents state of state machine which works on enumeration of characters (string).
    /// </summary>
    /// <typeparam name="TState">Type of target state.</typeparam>
    public interface IStringState<TState> : IStateMachineState<char, TState> 
    { }
}
