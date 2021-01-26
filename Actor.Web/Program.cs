using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Actor.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseOrleans(siloBuilder =>
            {
                //siloBuilder.UseLocalhostClustering()

                //persistant for clustering
                siloBuilder.UseAdoNetClustering(options =>
                 {
                     options.ConnectionString = "Integrated Security=true;Initial Catalog=Orleans1;Server=.";
                     options.Invariant = "System.Data.SqlClient";
                 })
                //important: different service should have different ports in dev env
                .ConfigureEndpoints(siloPort: 11112, gatewayPort: 30002)

                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "aspService";
                });
            });
    }
}
