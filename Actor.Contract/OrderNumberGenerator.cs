using Orleans;
using System.Threading.Tasks;

namespace Actor.Contract
{
    public interface IOrderNumberGenerator : IGrainWithStringKey
    {
        Task<int> GenerateOrderNumber();
    }
    public class OrderNumberGenerator : Grain, IOrderNumberGenerator
    {
        private int _current;
        public Task<int> GenerateOrderNumber()
        {
            _current++;
            System.Console.WriteLine($"Generated number is ${_current}");
            return Task.FromResult(_current);
        }
    }
}
