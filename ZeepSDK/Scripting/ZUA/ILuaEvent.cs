namespace ZeepSDK.Scripting.ZUA;

/// <summary>
/// Represents a Lua event that can be subscribed to or unsubscribed from.
/// </summary>
public interface ILuaEvent
{
    /// <summary>
    /// Gets the name of the Lua event.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Subscribes to the Lua event.
    /// </summary>
    void Subscribe();

    /// <summary>
    /// Unsubscribes from the Lua event.
    /// </summary>
    void Unsubscribe();
}
