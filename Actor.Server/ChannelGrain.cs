using Actor.Common;
using Orleans.Runtime;
using Orleans.Streams;

namespace Actor.Server;

public class ChannelGrain : Grain, IChannelGrain
{
    private IAsyncStream<ChatMsg> _stream = null!;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("chat");

        _stream = streamProvider.GetStream<ChatMsg>("ChatRoom", this.GetPrimaryKeyString());

        return base.OnActivateAsync(cancellationToken);
    }

    public async Task<StreamId> Join(string nickname)
    {
        await Console.Out.WriteLineAsync("www");

        await _stream.OnNextAsync(
            new ChatMsg(
                "System",
                $"{nickname} joins the chat '{this.GetPrimaryKeyString()}' ..."));

        return _stream.StreamId;
    }
}