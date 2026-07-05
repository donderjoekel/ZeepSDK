using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

/// <summary>
/// A local chat command alias that forwards to a primary command.
/// Alias entries appear in <see cref="ChatCommandRegistry.LocalChatCommands"/> alongside primary commands.
/// </summary>
[PublicAPI]
public interface ILocalChatCommandAlias : ILocalChatCommand
{
    /// <summary>
    /// The primary command this alias forwards to.
    /// </summary>
    ILocalChatCommand Target { get; }
}
