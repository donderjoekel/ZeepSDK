using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

[PublicAPI]
public interface IChatCommand
{
    string Prefix { get; }
    string Command { get; }
    string Description { get; }
}
