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
            var guid = this.GetPrimaryKey();

            var streamProvider = GetStreamProvider("SMSProvider");

            var stream = streamProvider.GetStream<int>(guid, "RANDOMDATA");

            await stream.SubscribeAsync<int>(async (data, token) =>
            {
                await Task.Delay(2000);
                Console.WriteLine("Data was recieved: " + data);
            });

            await base.OnActivateAsync();
        }
    }
}
