using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Gateway
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
            return builder;
        }
    }
}