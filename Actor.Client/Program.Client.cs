﻿using Actor.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

using var host = new HostBuilder()
    .UseOrleansClient(clientBuilder =>
    {
        clientBuilder.UseLocalhostClustering()
            .AddMemoryStreams("chat");
    })
    .Build();

var client = host.Services.GetRequiredService<IClusterClient>();

await StartAsync(host);

var room = client.GetGrain<IChannelGrain>("1");

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