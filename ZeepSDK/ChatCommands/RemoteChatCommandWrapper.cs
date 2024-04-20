namespace ZeepSDK.ChatCommands;

internal class RemoteChatCommandWrapper : IRemoteChatCommand
{
    private readonly RemoteChatCommandCallback _callback;

    public string Prefix
    {
        get;
    }

    public string Command
    {
        get;
    }

    public string Description
    {
        get;
    }

    public RemoteChatCommandWrapper(
        string prefix,
        string command,
        string description,
        RemoteChatCommandCallback callback
    )
    {
        Prefix = prefix;
        Command = command;
        Description = description;
        _callback = callback;
    }

    public void Handle(ulong playerId, string arguments)
    {
        _callback?.Invoke(playerId, arguments);
    }
}
