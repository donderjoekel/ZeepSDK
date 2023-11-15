namespace ZeepSDK.ChatCommands;

/// <summary>
/// The interface to use for a mixed chat command
/// </summary>
public interface IMixedChatCommand : ILocalChatCommand, IRemoteChatCommand
{
}
