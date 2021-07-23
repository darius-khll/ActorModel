using Orleans;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace Actor.Contract
{
    public interface ISmsStreamRecieverGrain : Orleans.IGrainWithIntegerKey
    {
    }

    [ImplicitStreamSubscription("RANDOMDATA")]
    public class SmsStreamRecieverGrain : Orleans.Grain, ISmsStreamRecieverGrain
    {
        public override async Task OnActivateAsync()
        {
            Console.WriteLine("sms grain is activated!");

            var guid = this.GetPrimaryKey();

            var streamProvider = GetStreamProvider("AzureQueueProvider");

            var stream = streamProvider.GetStream<int>(guid, "RANDOMDATA");

            await stream.SubscribeAsync<int>(async (data, token) =>
            {
                Console.WriteLine("Task started: " + data);
                await Task.Delay(2000);
                Console.WriteLine("Data was recieved: " + data + "\n---------");
            });

            //var a = await stream.GetAllSubscriptionHandles();
            //Console.WriteLine($"count = {a.Count} - Name = {a[0].StreamIdentity.Guid}");
            //await a[0].UnsubscribeAsync();

            await base.OnActivateAsync();
        }
    }
}
