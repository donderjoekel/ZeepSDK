namespace ZeepSDK.ChatCommands;

internal class LocalChatCommandWrapper : ILocalChatCommand
{
    private readonly LocalChatCommandCallbackDelegate callback;

    public string Prefix { get; }
    public string Command { get; }
    public string Description { get; }
    public string[] Aliases { get; }
    public string[] Arguments { get; }

    public LocalChatCommandWrapper(
        string prefix,
        string command,
        string description,
        LocalChatCommandCallbackDelegate callback,
        string[] aliases = null,
        string[] arguments = null
    )
    {
        Prefix = prefix;
        Command = command;
        Description = description;
        this.callback = callback;
        Aliases = aliases ?? [];
        Arguments = arguments ?? [];
    }

    public void Handle(string arguments)
    {
        callback?.Invoke(arguments);
    }
}
