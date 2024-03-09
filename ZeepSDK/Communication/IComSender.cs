using JetBrains.Annotations;

namespace ZeepSDK.Communication;

/// <summary>
/// A sender that can broadcast messages to all listening receivers
/// </summary>
[PublicAPI]
public interface IComSender
{
    /// <summary>
    /// Broadcasts a message to all listening receivers
    /// </summary>
    void SendMessage(string message);

    /// <summary>
    /// Converts the given object to a json string and broadcasts it to all listening receivers
    /// </summary>
    /// <param name="obj">the data to convert to json</param>
    void SendAsJson(object obj);
}
