namespace ZeepSDK.Utilities.Override;

/// <summary>
/// Specifies when a condition should be evaluated in the Unity lifecycle.
/// Used by <see cref="ConditionalOverrideLayer{T}"/> to determine when to check its condition.
/// </summary>
public enum ConditionTickType
{
    /// <summary>
    /// The condition is evaluated during Unity's Update method, once per frame.
    /// </summary>
    Update,
    
    /// <summary>
    /// The condition is evaluated during Unity's FixedUpdate method, at fixed time intervals.
    /// </summary>
    FixedUpdate,
    
    /// <summary>
    /// The condition is evaluated during Unity's LateUpdate method, after all Update methods have been called.
    /// </summary>
    LateUpdate,
    
    /// <summary>
    /// The condition is evaluated during Unity's OnGUI method, multiple times per frame for GUI events.
    /// </summary>
    OnGUI
}
