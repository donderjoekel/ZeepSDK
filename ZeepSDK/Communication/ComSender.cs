using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using Newtonsoft.Json;
using ZeepSDK.Utilities;

namespace ZeepSDK.Communication;

internal class ComSender : IComSender
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger<ComSender>();

    private readonly BaseUnityPlugin plugin;

    public ComSender(BaseUnityPlugin plugin)
    {
        this.plugin = plugin;
    }

    public void SendMessage(string message)
    {
        List<IComReceiver> comReceivers = ReceiverRepository.GetReceivers(plugin.Info.Metadata.GUID);

        foreach (IComReceiver comReceiver in comReceivers)
        {
            try
            {
                if (comReceiver == null)
                {
                    logger.LogWarning("Skipping null receiver");
                    continue;
                }

                comReceiver.ProcessMessage(message);
            }
            catch (Exception e)
            {
                logger.LogError("Error while sending message: " + e);
            }
        }
    }

    public void SendAsJson(object obj)
    {
        string json = JsonConvert.SerializeObject(obj);
        SendMessage(json);
    }
}
