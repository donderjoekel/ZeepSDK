using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

/// <summary>
/// The interface to use for remote chat commands
/// </summary>
[PublicAPI]
public interface IRemoteChatCommand : IChatCommand
{
    /// <summary>
    /// This method gets called whenever the chat command gets used by another user
    /// </summary>
    /// <param name="playerId">The steam id of the user</param>
    /// <param name="arguments">Any other text the user might have put after the command</param>
    void Handle(ulong playerId, string arguments);
}
