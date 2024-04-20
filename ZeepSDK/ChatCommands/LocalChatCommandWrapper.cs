namespace ZeepSDK.ChatCommands;

internal class LocalChatCommandWrapper : ILocalChatCommand
{
    private readonly LocalChatCommandCallback _callback;

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

    public LocalChatCommandWrapper(
        string prefix,
        string command,
        string description,
        LocalChatCommandCallback callback
    )
    {
        Prefix = prefix;
        Command = command;
        Description = description;
        _callback = callback;
    }

    public void Handle(string arguments)
    {
        _callback?.Invoke(arguments);
    }
}
