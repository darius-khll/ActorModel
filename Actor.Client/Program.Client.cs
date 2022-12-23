using Actor.Client;
using Actor.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;
using Spectre.Console;

using var host = new HostBuilder()
    .UseOrleansClient(clientBuilder =>
    {
        clientBuilder.UseLocalhostClustering()
            .AddMemoryStreams(OrleansConstants.StreamProvider);
    })
    .Build();

var client = host.Services.GetRequiredService<IClusterClient>();

await StartAsync(host);

var streamId = StreamId.Create(OrleansConstants.Stream, OrleansConstants.Channel);
var stream =
    client
        .GetStreamProvider(OrleansConstants.StreamProvider)
        .GetStream<ChatMsg>(streamId);
await stream.SubscribeAsync(new StreamObserver(OrleansConstants.Channel));


var room = client.GetGrain<IChannelGrain>(OrleansConstants.Channel);
do
{
    await room.Join("wow");
    Console.ReadKey();
} while (true);

static Task StartAsync(IHost host) =>
    AnsiConsole.Status().StartAsync("Connecting to server", async ctx =>
    {
        await host.StartAsync();
        ctx.Status = "Connected!";
    });