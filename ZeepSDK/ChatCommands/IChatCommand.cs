using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

/// <summary>
/// The base interface for all chat commands
/// </summary>
[PublicAPI]
public interface IChatCommand
{
    /// <summary>
    /// The prefix of this chat command
    /// </summary>
    string Prefix { get; }

    /// <summary>
    /// The keyword of this chat command
    /// </summary>
    string Command { get; }

    /// <summary>
    /// The description of this chat command
    /// </summary>
    string Description { get; }
}
