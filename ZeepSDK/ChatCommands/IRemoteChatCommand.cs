using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

[PublicAPI]
public interface IRemoteChatCommand : IChatCommand
{
    void Handle(ulong playerId, string arguments);
}
