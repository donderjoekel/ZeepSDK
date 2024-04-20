namespace ZeepSDK.ChatCommands;

internal class MixedChatCommandWrapper : MixedChatCommandBase
{
    private readonly MixedChatCommandCallback _callback;

    public override string Prefix
    {
        get;
    }

    public override string Command
    {
        get;
    }

    public override string Description
    {
        get;
    }

    public MixedChatCommandWrapper(
        string prefix,
        string command,
        string description,
        MixedChatCommandCallback callback
    )
    {
        Prefix = prefix;
        Command = command;
        Description = description;
        _callback = callback;
    }

    protected override void Handle(bool isLocal, ulong playerId, string arguments)
    {
        _callback?.Invoke(isLocal, playerId, arguments);
    }
}
