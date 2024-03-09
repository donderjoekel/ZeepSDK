using System;
using BepInEx.Logging;
using ZeepSDK.Utilities;

namespace ZeepSDK.Communication;

internal class ComReceiver : IComReceiver
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger<ComReceiver>();

    public Guid Guid { get; }
    public string ModIdentifier { get; }

    public event MessageReceivedDelegate MessageReceived;

    public ComReceiver(Guid guid, string modIdentifier)
    {
        Guid = guid;
        ModIdentifier = modIdentifier;
    }

    public void ProcessMessage(string message)
    {
        try
        {
            MessageReceived?.Invoke(this, message);
        }
        catch (Exception e)
        {
            logger.LogError("Error while processing message: " + e);
        }
    }

    public bool Equals(IComReceiver other)
    {
        return other != null && Guid.Equals(other.Guid);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != typeof(IComReceiver)) return false;
        return Equals((IComReceiver)obj);
    }

    public override int GetHashCode()
    {
        return Guid.GetHashCode();
    }

    public static bool operator ==(ComReceiver left, ComReceiver right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ComReceiver left, ComReceiver right)
    {
        return !Equals(left, right);
    }
}
