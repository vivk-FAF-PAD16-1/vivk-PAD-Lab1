using System.Threading;
using System.Threading.Tasks;
using Discovery.Listener;
using Discovery.Router;
using Discovery.Storage;

namespace Discovery
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var storage = new RouteStorage() as IStorage;
            
            var registratorRouter = new RegistratorRouter(storage) as IRouter;
            
            var prefixes0 = new[] { "http://localhost:40404/" };
            var registratorListener = new AsyncListener(prefixes0, registratorRouter) as IAsyncListener;
            registratorListener.Schedule();

            var discoveryRouter = new DiscoveryRouter(storage);
            
            var prefexes1 = new[] { "http://localhost:40405/" };
            var discoveryListener = new AsyncListener(prefexes1, discoveryRouter) as IAsyncListener;
            discoveryListener.Schedule();
            
            Thread.Sleep(Timeout.InfiniteTimeSpan);          
        }
    }
}