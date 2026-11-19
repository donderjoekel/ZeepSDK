using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZeepSDK.Communication;
using Xunit;

namespace ZeepSDK.Tests;

public class CommunicationTests
{
    private readonly ComRegistry registry = new();

    private ComReceiver CreateReceiver(string sourceModGuid, string topic = null)
    {
        ComReceiver receiver = new(registry, sourceModGuid, topic);
        registry.Add(receiver);
        return receiver;
    }

    [Fact]
    public void DeliversMessageToReceiversOfSendingMod()
    {
        ComSender sender = new(registry, "mod-a");
        ComReceiver receiver = CreateReceiver("mod-a");
        ComReceiver other = CreateReceiver("mod-b");
        List<ComMessage> received = new();
        receiver.MessageReceived += (_, message) => received.Add(message);
        other.MessageReceived += (_, _) => throw new Exception("should not be called");

        sender.Send("topic", "payload");

        ComMessage message = Assert.Single(received);
        Assert.Equal("mod-a", message.SourceModGuid);
        Assert.Equal("topic", message.Topic);
        Assert.Equal("payload", message.Payload);
    }

    [Fact]
    public void TopicFilteredReceiverOnlyGetsMatchingTopic()
    {
        ComSender sender = new(registry, "mod");
        ComReceiver filtered = CreateReceiver("mod", "wanted");
        ComReceiver all = CreateReceiver("mod");
        List<string> filteredTopics = new();
        List<string> allTopics = new();
        filtered.MessageReceived += (_, message) => filteredTopics.Add(message.Topic);
        all.MessageReceived += (_, message) => allTopics.Add(message.Topic);

        sender.Send("wanted", null);
        sender.Send("other", null);

        Assert.Equal(new[] { "wanted" }, filteredTopics);
        Assert.Equal(new[] { "wanted", "other" }, allTopics);
    }

    [Fact]
    public void DisposedReceiverStopsReceiving()
    {
        ComSender sender = new(registry, "mod");
        ComReceiver receiver = CreateReceiver("mod");
        int count = 0;
        receiver.MessageReceived += (_, _) => count++;

        sender.Send("t", "1");
        receiver.Dispose();
        sender.Send("t", "2");

        Assert.Equal(1, count);
        Assert.True(receiver.IsDisposed);
        Assert.Empty(registry.GetReceivers("mod"));
    }

    [Fact]
    public void ReceiverCanUnsubscribeAndSubscribeDuringDispatch()
    {
        ComSender sender = new(registry, "mod");
        ComReceiver first = CreateReceiver("mod");
        ComReceiver second = CreateReceiver("mod");
        int secondCount = 0;
        first.MessageReceived += (receiver, _) =>
        {
            receiver.Dispose();
            CreateReceiver("mod");
        };
        second.MessageReceived += (_, _) => secondCount++;

        Exception exception = Record.Exception(() => sender.Send("t", null));

        Assert.Null(exception);
        Assert.Equal(1, secondCount);
    }

    [Fact]
    public void ReceiverDisposedByEarlierReceiverDuringDispatchIsSkipped()
    {
        ComSender sender = new(registry, "mod");
        ComReceiver first = CreateReceiver("mod");
        ComReceiver second = CreateReceiver("mod");
        int secondCount = 0;
        first.MessageReceived += (_, _) => second.Dispose();
        second.MessageReceived += (_, _) => secondCount++;

        sender.Send("t", null);

        Assert.Equal(0, secondCount);
    }

    [Fact]
    public void ThrowingHandlerDoesNotAffectSenderOrOtherHandlers()
    {
        ComSender sender = new(registry, "mod");
        ComReceiver receiver = CreateReceiver("mod");
        ComReceiver other = CreateReceiver("mod");
        int count = 0;
        receiver.MessageReceived += (_, _) => throw new InvalidOperationException();
        receiver.MessageReceived += (_, _) => count++;
        other.MessageReceived += (_, _) => count++;

        Exception exception = Record.Exception(() => sender.Send("t", null));

        Assert.Null(exception);
        Assert.Equal(2, count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SendRejectsMissingTopic(string topic)
    {
        ComSender sender = new(registry, "mod");

        Assert.Throws<ArgumentException>(() => sender.Send(topic, "payload"));
    }

    [Fact]
    public void JsonRoundTrips()
    {
        ComSender sender = new(registry, "mod");
        ComReceiver receiver = CreateReceiver("mod");
        ComMessage received = null;
        receiver.MessageReceived += (_, message) => received = message;

        sender.SendJson("data", new Payload { Name = "zeep", Value = 42 });

        Payload payload = received.ReadJson<Payload>();
        Assert.Equal("zeep", payload.Name);
        Assert.Equal(42, payload.Value);
    }

    [Fact]
    public void TryReadJsonReturnsFalseForInvalidJson()
    {
        ComSender sender = new(registry, "mod");
        ComReceiver receiver = CreateReceiver("mod");
        ComMessage received = null;
        receiver.MessageReceived += (_, message) => received = message;

        sender.Send("data", "not json {");

        Assert.False(received.TryReadJson(out Payload payload));
        Assert.Null(payload);
    }

    [Fact]
    public async Task ConcurrentSubscribeAndSendDoesNotThrow()
    {
        ComSender sender = new(registry, "mod");

        Task subscribe = Task.Run(() =>
        {
            for (int i = 0; i < 10_000; i++)
            {
                CreateReceiver("mod").Dispose();
            }
        });
        Task send = Task.Run(() =>
        {
            for (int i = 0; i < 10_000; i++)
            {
                sender.Send("t", null);
            }
        });

        await Task.WhenAll(subscribe, send);
        Assert.Empty(registry.GetReceivers("mod"));
    }

    private sealed class Payload
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
