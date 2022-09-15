using System.Threading;
using Gateway.Listener;
using Gateway.Router;
using Gateway.Storage;

namespace Gateway
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

            var gatewayRouter = new GatewayRouter(storage);
            
            var prefexes1 = new[] { "http://localhost:40405/" };
            var gatewayListener = new AsyncListener(prefexes1, gatewayRouter) as IAsyncListener;
            gatewayListener.Schedule();
            
            Thread.Sleep(1000000);
            
            registratorListener.Stop();
        }
    }
}