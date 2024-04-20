namespace ZeepSDK.ChatCommands;

/// <summary>
/// A base class that can be used for implementing a mixed chat command
/// </summary>
public abstract class MixedChatCommandBase : IMixedChatCommand
{
    /// <inheritdoc />
    public abstract string Prefix
    {
        get;
    }

    /// <inheritdoc />
    public abstract string Command
    {
        get;
    }

    /// <inheritdoc />
    public abstract string Description
    {
        get;
    }

    void IRemoteChatCommand.Handle(ulong playerId, string arguments)
    {
        Handle(false, playerId, arguments);
    }

    void ILocalChatCommand.Handle(string arguments)
    {
        Handle(true, 0, arguments);
    }

    /// <summary>
    /// The method that gets called whenever the chat command gets used
    /// </summary>
    /// <param name="isLocal">Is this command invoked by a local user or not</param>
    /// <param name="playerId">0 if was invoked by the local user, otherwise the steam id of the invoking user</param>
    /// <param name="arguments">Any other text the user might have put after the command</param>
    protected abstract void Handle(bool isLocal, ulong playerId, string arguments);
}
