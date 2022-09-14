using System.Threading;
using Gateway.Listener;
using Gateway.Router;

namespace Gateway
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var registratorRouter = new RegistratorRouter() as IRouter;
            
            var prefixes = new[] { "http://localhost:40404/" };
            var registratorListener = new AsyncListener(prefixes, registratorRouter) as IAsyncListener;
            registratorListener.Schedule();
            
            Thread.Sleep(100000);
            
            registratorListener.Stop();
        }
    }
}