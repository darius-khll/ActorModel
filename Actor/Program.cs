using Actor.Contract;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
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
            var builder = new SiloHostBuilder()
                //localDev
                //.UseLocalhostClustering()

                //persistant for clustering
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = "Integrated Security=true;Initial Catalog=Orleans1;Server=.";
                    options.Invariant = "System.Data.SqlClient";
                })
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)

                //transaction
                .AddMemoryGrainStorageAsDefault()
                .UseTransactions()

                //storage
                .AddAdoNetGrainStorage("profileStore", options =>
                {
                    //Some query should be executed in order to handler associated tables and records to be stored in the db
                    options.ConnectionString = "Integrated Security=true;Initial Catalog=Orleans1;Server=.";
                    options.UseJsonFormat = true;
                })

                //Streaming
                .AddSimpleMessageStreamProvider("SMSProvider", (options) => options.OptimizeForImmutableData = false)
                .AddMemoryGrainStorage("PubSubStore")

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
