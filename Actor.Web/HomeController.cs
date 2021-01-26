using Actor.Contract;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace Actor.Web
{
    public class HomeController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public HomeController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        public async Task<int> Index(string s = "key")
        {
            var orderNumberGenerator = _clusterClient.GetGrain<IOrderNumberGenerator>(s);
            var orderNumber = await orderNumberGenerator.GenerateOrderNumber();

            return orderNumber;
        }
    }
}
