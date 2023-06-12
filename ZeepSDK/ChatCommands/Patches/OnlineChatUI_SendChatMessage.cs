using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.ChatCommands.Patches;

[HarmonyPatch(typeof(OnlineChatUI), nameof(OnlineChatUI.SendChatMessage))]
internal class OnlineChatUI_SendChatMessage
{
    [UsedImplicitly]
    private static bool Prefix(string message)
    {
        bool executedCustomCommand = false;

        foreach (ILocalChatCommand localChatCommand in ChatCommandRegistry.LocalChatCommands)
        {
            if (!message.StartsWith(localChatCommand.Prefix))
                continue;

            string messageWithoutPrefix = message[1..];

            if (!messageWithoutPrefix.StartsWith(localChatCommand.Command))
                continue;

            string arguments = messageWithoutPrefix[localChatCommand.Command.Length..].Trim();

            localChatCommand.Handle(arguments);
            executedCustomCommand = true;
        }

        return !executedCustomCommand;
    }
}
