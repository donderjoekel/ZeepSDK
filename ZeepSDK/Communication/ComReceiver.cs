using System;
using BepInEx.Logging;
using ZeepSDK.Utilities;

namespace ZeepSDK.Communication;

internal class ComReceiver : IComReceiver
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger<ComReceiver>();

    public Guid Identifier
    {
        get;
    }

    public string ModIdentifier
    {
        get;
    }

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    public ComReceiver(Guid guid, string modIdentifier)
    {
        Identifier = guid;
        ModIdentifier = modIdentifier;
    }

    public void ProcessMessage(string message)
    {
        try
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(this, message));
        }
        catch (Exception e)
        {
            _logger.LogError("Error while processing message: " + e);
        }
    }

    public bool Equals(IComReceiver other)
    {
        return other != null && Identifier.Equals(other.Identifier);
    }

    public override bool Equals(object obj)
    {
        return obj is not null && (ReferenceEquals(this, obj) ||
                                   (obj.GetType() == typeof(IComReceiver) && Equals((IComReceiver)obj)));
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
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
