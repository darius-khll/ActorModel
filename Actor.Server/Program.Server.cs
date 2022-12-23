using Actor.Common;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder
            .UseLocalhostClustering()
            .AddMemoryGrainStorage("PubSubStore")
            .AddMemoryStreams(OrleansConstants.StreamProvider);
    })
    .RunConsoleAsync();
