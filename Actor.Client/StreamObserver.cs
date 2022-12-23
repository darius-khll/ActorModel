using Actor.Common;
using Orleans.Streams;

namespace Actor.Client;

public sealed class StreamObserver : IAsyncObserver<ChatMsg>
{
    private readonly string _roomName;

    public StreamObserver(string roomName) => _roomName = roomName;

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex)
    {
        Console.WriteLine("err");
        return Task.CompletedTask;
    }

    public Task OnNextAsync(ChatMsg item, StreamSequenceToken? token = null)
    {
        Console.WriteLine("Message recieved");

        return Task.CompletedTask;
    }
}

//public interface IMessagingGrain : IGrainWithIntegerKey
//{
//}

//[ImplicitStreamSubscription("MessagingGrain")]
//public class MessagingGrain : Grain, IMessagingGrain
//{
//    public override async Task OnActivateAsync(CancellationToken cancellationToken)
//    {
//        var guid = this.GetPrimaryKey();

//        var streamProvider = this.GetStreamProvider("SMSProvider");

//        var stream = streamProvider.GetStream<ChatMsg>("MessagingGrain", guid);

//        await stream.SubscribeAsync(async (data, token) =>
//        {
//            await Console.Out.WriteLineAsync($"Message: {data.Author}");
//        });

//        await base.OnActivateAsync(cancellationToken);
//    }
//}