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
            
            var prefixes = new[] { "http://localhost:40404/" };
            var registratorListener = new AsyncListener(prefixes, registratorRouter) as IAsyncListener;
            registratorListener.Schedule();
            
            Thread.Sleep(100000);
            
            registratorListener.Stop();
        }
    }
}