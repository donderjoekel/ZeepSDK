# Communicating Between Mods

The Communication API lets one mod send messages that other mods can listen to, without either mod needing a compile-time reference to the other.

## Overview

- A mod creates an `IComSender` for itself and sends messages on named **topics**.
- Other mods create an `IComReceiver` for that mod's GUID, optionally filtered on a single topic.
- Each message is a `ComMessage` with the sender's GUID, the topic, and a string payload.

## Sending messages

```csharp
using BepInEx;
using ZeepSDK.Communication;

[BepInPlugin("com.example.timer", "Timer", "1.0.0")]
public class TimerMod : BaseUnityPlugin
{
    private IComSender sender;

    private void Awake()
    {
        sender = CommunicationApi.CreateSender(this);
    }

    private void OnFinished(float time)
    {
        // Plain string payload
        sender.Send("finished", time.ToString(System.Globalization.CultureInfo.InvariantCulture));

        // Or serialize an object as json
        sender.SendJson("split", new SplitData { Checkpoint = 3, Time = time });
    }
}
```

## Receiving messages

```csharp
using BepInEx;
using ZeepSDK.Communication;

[BepInPlugin("com.example.overlay", "Overlay", "1.0.0")]
[BepInDependency("com.example.timer", BepInDependency.DependencyFlags.SoftDependency)]
public class OverlayMod : BaseUnityPlugin
{
    private IComReceiver splitReceiver;

    private void Awake()
    {
        // Only receive messages on the "split" topic. Omit the topic to receive everything.
        splitReceiver = CommunicationApi.CreateReceiver("com.example.timer", "split");
        splitReceiver.MessageReceived += OnSplit;
    }

    private void OnSplit(IComReceiver receiver, ComMessage message)
    {
        if (message.TryReadJson(out SplitData data))
        {
            Logger.LogInfo($"Checkpoint {data.Checkpoint}: {data.Time}");
        }
    }

    private void OnDestroy()
    {
        splitReceiver.Dispose();
    }
}
```

Both mods define their own `SplitData` class with matching property names; only the json crosses the boundary.

## Things to know

- **Delivery is synchronous.** `Send` invokes every matching receiver on the calling thread before returning. If you send from a background thread, your handlers run on that thread too.
- **No history.** Receivers only get messages sent after they were created. Mods load one after another, so a message sent from a sender's `Awake` can be missed by mods that load later. Create receivers in `Awake`, and send messages from game events rather than during plugin startup.
- **Errors are isolated.** An exception thrown by a handler is logged and does not reach the sender or other handlers.
- **Disposing is safe anywhere**, including inside a `MessageReceived` handler.
