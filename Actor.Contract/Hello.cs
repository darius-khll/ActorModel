using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Actor.Contract
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<int> GetHello();
    }

    public class ProfileState
    {
        public string Name { get; set; }
        public List<int> Nums { get; set; } = new List<int>();
    }

    public class HelloGrain : Orleans.Grain, IHello
    {

        private readonly IPersistentState<ProfileState> _profile;

        public HelloGrain([PersistentState("profile", "profileStore")] IPersistentState<ProfileState> profile)
        {
            _profile = profile;
        }

        async Task<int> IHello.GetHello()
        {
            Console.WriteLine($"{_profile.State.Name} - {_profile.State.Nums.Count}\n-------");

            _profile.State.Name = "changed";
            _profile.State.Nums.Add(1);

            await _profile.WriteStateAsync();

            return 1;
        }
    }
}
