namespace ZeepSDK.ChatCommands;

internal sealed class LocalChatCommandAlias : ILocalChatCommandAlias
{
    public ILocalChatCommand Target { get; }
    public string Prefix { get; }
    public string Command { get; }
    public string Description { get; }

    public LocalChatCommandAlias(ILocalChatCommand target, string alias)
    {
        Target = target;
        Prefix = target.Prefix;
        Command = alias;
        Description = target.Description;
    }

    public void Handle(string arguments) => Target.Handle(arguments);
}
