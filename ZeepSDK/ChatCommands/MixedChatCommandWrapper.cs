using ZeepSDK.ChatCommands.Commands;

namespace ZeepSDK.ChatCommands;

internal class MixedChatCommandWrapper : MixedChatCommandBase
{
    private readonly MixedChatCommandCallbackDelegate callback;

    public override string Prefix { get; }
    public override string Command { get; }
    public override string Description { get; }
    public override string[] Aliases { get; }
    public override string[] Arguments { get; }

    public MixedChatCommandWrapper(
        string prefix,
        string command,
        string description,
        MixedChatCommandCallbackDelegate callback
    )
    {
        Prefix = prefix;
        Command = command;
        Description = description;
        this.callback = callback;
        Aliases = [];
        Arguments = [];
    }

    protected override void Handle(bool isLocal, ulong playerId, string arguments)
    {
        callback?.Invoke(isLocal, playerId, arguments);
    }
}
