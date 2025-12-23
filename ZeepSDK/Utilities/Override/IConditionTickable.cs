namespace ZeepSDK.Utilities.Override;

/// <summary>
/// Represents an object that can be ticked by the <see cref="ConditionTicker"/> at regular intervals.
/// Implementations specify when they should be ticked via the <see cref="TickType"/> property.
/// </summary>
public interface IConditionTickable
{
    /// <summary>
    /// Gets the tick type that determines when this object should be ticked.
    /// </summary>
    ConditionTickType TickType { get; }
    
    /// <summary>
    /// Performs the tick operation. This method is called periodically by the <see cref="ConditionTicker"/>
    /// according to the <see cref="TickType"/>.
    /// </summary>
    void Tick();
}
