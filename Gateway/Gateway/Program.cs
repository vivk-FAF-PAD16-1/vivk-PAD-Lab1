using System.Threading;
using Gateway.Listener;
using Gateway.Router;

namespace Gateway
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			const string discoveryUri = "http://localhost:40405/";
			
			var gatewayRouter = new GatewayRouter(discoveryUri);
			
			var prefixes0 = new[] { "http://localhost:40300/" };
			var gatewayListener = new AsyncListener(prefixes0, gatewayRouter) as IAsyncListener;
			gatewayListener.Schedule();

			Thread.Sleep(Timeout.InfiniteTimeSpan);
		}
	}
}