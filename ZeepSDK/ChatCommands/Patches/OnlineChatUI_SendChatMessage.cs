using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using ZeepSDK.Utilities;

namespace ZeepSDK.ChatCommands.Patches;

[HarmonyPatch(typeof(OnlineChatUI), nameof(OnlineChatUI.SendChatMessage))]
internal class OnlineChatUI_SendChatMessage
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(OnlineChatUI_SendChatMessage));

    [UsedImplicitly]
    private static bool Prefix(string message)
    {
        try
        {
            bool executedCustomCommand = false;

            List<ILocalChatCommand> matchingCommands = ChatCommandRegistry.LocalChatCommands
                .Where(x => ChatCommandUtilities.MatchesCommand(message, x))
                .OrderByDescending(x=>x.Command.Length)
                .ToList();

            if (matchingCommands.Count > 0)
            {
                executedCustomCommand = true;
                ProcessLocalChatCommand(matchingCommands.First(), message);
            }

            return !executedCustomCommand;
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(Prefix)}: " + e);
            return true;
        }
    }
    
    

    private static bool ProcessLocalChatCommand(ILocalChatCommand localChatCommand, string message)
    {
        string arguments = ChatCommandUtilities.GetArguments(message, localChatCommand);

        try
        {
            localChatCommand.Handle(arguments);
        }
        catch (Exception e)
        {
            ManualLogSource manualLogSource = LoggerFactory.GetLogger(localChatCommand);
            manualLogSource.LogError($"Unhandled exception in {localChatCommand.GetType().Name}: " + e);
            return false;
        }

        return true;
    }
}
