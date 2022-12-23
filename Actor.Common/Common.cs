using Orleans.Runtime;

namespace Actor.Common;

[GenerateSerializer]
public record class ChatMsg(
    string? Author,
    string Text)
{
    [Id(0)]
    public string Author { get; init; } = Author ?? "Alexey";

    [Id(1)]
    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;
}

public interface IChannelGrain : IGrainWithStringKey
{
    Task<StreamId> Join(string nickname);
}


public static class OrleansConstants
{
    public const string StreamProvider = "chat";
    public const string Stream = "ChatRoom";
    public const string Channel = "1";
}
