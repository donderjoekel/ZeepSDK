namespace ZeepSDK.ChatCommands;

internal class RemoteChatCommandWrapper : IRemoteChatCommand
{
    private readonly RemoteChatCommandCallbackDelegate callback;

    public string Prefix { get; }
    public string Command { get; }
    public string Description { get; }
    public string[] Aliases { get; }
    public string[] Arguments { get; }

    public RemoteChatCommandWrapper(
        string prefix,
        string command,
        string description,
        RemoteChatCommandCallbackDelegate callback
    )
    {
        Prefix = prefix;
        Command = command;
        Description = description;
        this.callback = callback;
        Aliases = [];
        Arguments = [];
    }

    public void Handle(ulong playerId, string arguments)
    {
        callback?.Invoke(playerId, arguments);
    }
}
