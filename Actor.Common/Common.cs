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
