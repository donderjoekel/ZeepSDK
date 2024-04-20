using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

/// <summary>
/// The interface to use for local chat commands
/// </summary>
[PublicAPI]
public interface ILocalChatCommand : IChatCommand
{
    /// <summary>
    /// This method gets called whenever the chat command gets used
    /// </summary>
    /// <param name="arguments">Any other text the user might have put after the command</param>
    void Handle(string arguments);
}
