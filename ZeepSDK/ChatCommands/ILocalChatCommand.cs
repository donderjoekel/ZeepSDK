using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands;

[PublicAPI]
public interface ILocalChatCommand : IChatCommand
{
    void Handle(string arguments);
}
