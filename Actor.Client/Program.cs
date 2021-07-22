using Actor.Contract;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;

namespace Actor.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await DoCall();
        }

        static async Task DoCall()
        {
            int count = -1;

            using (var client = await ConnectClient())
            {
                for (int i = 0; i <= 500; i++)
                {

                    try
                    {
                        //await ATM(client);
                        // count = await GetClientWork(client);
                        await SendSms(client, i);
                        //await SendEventSourcing(client, i);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            Console.WriteLine($"Count number: {count}");
            Console.ReadKey();
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                //localDev
                //.UseLocalhostClustering()

                //Clustering
                //https://github.com/dotnet/orleans/blob/ba30bbb2155168fc4b9f190727220583b9a7ae4c/src/OrleansSQLUtils/CreateOrleansTables_SqlServer.sql
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = "Integrated Security=true;Initial Catalog=Orleans1;Server=.";
                    options.Invariant = "System.Data.SqlClient";
                })

                //Streaming
                .AddSimpleMessageStreamProvider("SMSProvider", (options) =>
                {
                    options.FireAndForgetDelivery = true;
                })


                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics1";
                })
                .Build();

            await client.Connect();
            return client;
        }

        private static async Task<int> GetClientWork(IClusterClient client)
        {
            var gen = client.GetGrain<IOrderNumberGenerator>("key");
            Console.WriteLine(await gen.GenerateOrderNumber());

            var friend = client.GetGrain<IHello>(1);
            return await friend.GetHello();
        }

        private static async Task ATM(IClusterClient client)
        {
            try
            {
                IATMGrain atm = client.GetGrain<IATMGrain>(0);
                var from = "A";
                var to = "B";

                await atm.Transfer(from, to, 100);

                var fromBalance = await client.GetGrain<IAccountGrain>(from).GetBalance();
                var toBalance = await client.GetGrain<IAccountGrain>(to).GetBalance();

                Console.WriteLine(fromBalance);
                Console.WriteLine(toBalance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static async Task SendSms(IClusterClient client, int i)
        {
            Console.WriteLine($"{i} - please enter to send more sms:");
            Console.ReadKey();

            var guid = Guid.NewGuid();

            var streamProvider = client.GetStreamProvider("SMSProvider");

            var stream = streamProvider.GetStream<int>(guid, "RANDOMDATA");

            await stream.OnNextAsync(i);
        }

        private static async Task SendEventSourcing(IClusterClient client, int i)
        {
            Console.WriteLine($"{i} - please enter to send more sms:");
            Console.ReadKey();

            var guid = Guid.Parse("ef0874b9-4696-4493-bb83-4b184865b958");

            var gen = client.GetGrain<IShipment>(guid);

            await gen.Pickup();
            Console.WriteLine(await gen.GetStatus());

            await gen.Deliver();
            Console.WriteLine(await gen.GetStatus());
        }
    }
}
