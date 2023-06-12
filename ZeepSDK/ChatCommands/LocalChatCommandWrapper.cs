namespace ZeepSDK.ChatCommands;

internal class LocalChatCommandWrapper : ILocalChatCommand
{
    private readonly LocalChatCommandCallbackDelegate callback;

    public string Prefix { get; }
    public string Command { get; }
    public string Description { get; }

    public LocalChatCommandWrapper(
        string prefix,
        string command,
        string description,
        LocalChatCommandCallbackDelegate callback
    )
    {
        Prefix = prefix;
        Command = command;
        Description = description;
        this.callback = callback;
    }

    public void Handle(string arguments)
    {
        callback?.Invoke(arguments);
    }
}
