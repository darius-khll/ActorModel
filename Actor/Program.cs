using Actor.Contract;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.Streams.AzureQueue;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Actor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = await StartSilo();
            Console.WriteLine("\n\n Press Enter to terminate...\n\n");
            Console.ReadLine();

            await host.StopAsync();
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var sP = new Random().Next(11113, 19299);
            var gP = new Random().Next(30213, 39213);

            var builder = new SiloHostBuilder()
                //localDev
                //.UseLocalhostClustering()

                //IMPORTANT
                //https://github.com/dotnet/orleans/blob/ba30bbb2155168fc4b9f190727220583b9a7ae4c/src/OrleansSQLUtils/CreateOrleansTables_SqlServer.sql
                //https://dotnet.github.io/orleans/docs/host/configuration_guide/adonet_configuration.html
                //persistant for clustering
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = "Integrated Security=true;Initial Catalog=Orleans1;Server=.";
                    options.Invariant = "System.Data.SqlClient";
                })
                .ConfigureEndpoints(siloPort: sP, gatewayPort: gP)

                //transaction
                .AddMemoryGrainStorageAsDefault()
                .UseTransactions()

                //storage
                //.AddAdoNetGrainStorage("profileStore", options =>
                //{
                //    //IMPORTANT
                //    //Some query should be executed in order to handler associated tables and records to be stored in the db
                //    options.ConnectionString = "Integrated Security=true;Initial Catalog=Orleans1;Server=.";
                //    options.UseJsonFormat = true;
                //})


                //Streaming
                //Fire and forget should be configured in client instead
                //.AddSimpleMessageStreamProvider("SMSProvider", (options) =>
                //{
                //    options.FireAndForgetDelivery = true;
                //})
                //docker run -p 8888:8888 mcr.microsoft.com/azure-storage/azurite azurite-queue --queueHost 0.0.0.0 --queuePort 8888
                .AddAzureQueueStreams("AzureQueueProvider", optionsBuilder =>
                {
                    //optionsBuilder.ConfigurePullingAgent(ob => ob.Configure(options => {
                    //    options.GetQueueMsgsTimerPeriod = TimeSpan.FromSeconds(1);
                    //}));

                    optionsBuilder.ConfigureAzureQueue(ob =>
                    {
                        ob.Configure(options =>
                        {
                            //find connection string on azure storage explorer
                            options.ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:8888/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
                            options.QueueNames = new List<string> { "" +
                                    "orleansbasic1-azurequeueprovider-0"};
                        });
                    });
                })
                //.AddMemoryGrainStorage("PubSubStore")
                .AddAdoNetGrainStorage("PubSubStore", optionsBuilder => //It MUST be "PubSubStore"
                {
                    optionsBuilder.ConnectionString = "Integrated Security=true;Initial Catalog=Orleans1;Server=.";
                    optionsBuilder.Invariant = "System.Data.SqlClient";
                    optionsBuilder.UseJsonFormat = true;
                })


                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
