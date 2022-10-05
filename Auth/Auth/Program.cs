using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Auth.Common;
using Auth.Counter;
using Auth.Discovery;
using Auth.Listener;
using Auth.Router;

namespace Auth
{
	internal static class Program
	{
		public const bool Independent = true;
		
		private const string ConfigurationPath = "../../Resources/configuration.json";

		public static async Task Main(string[] args)
		{
			var directoryAbsolutePath = AppDomain.CurrentDomain.BaseDirectory;
			var configurationAbsolutePath = Path.Combine(
				directoryAbsolutePath, ConfigurationPath);
			
			var configurator = new Configurator(configurationAbsolutePath);
			var configurationData = configurator.Load();

			var counter = new SyncCounter(configurationData.MaxCount) as ICounter;

			var authRouter = new AuthRouter(ref configurationData, counter) as IRouter;

			var authListener = new AsyncListener(
				configurationData.AuthPrefixes,
				authRouter);
			authListener.Schedule();

			if (!configurationData.Independent)
			{
				var registrator = new Registrator(
					configurationData.DiscoveryUri, 
					configurationData.Routes) as IRegistrator;
			
				var ok = await registrator.Register();
				if (!ok)
				{
					authListener.Stop();
					return;
				}
			}

			Thread.Sleep(Timeout.InfiniteTimeSpan);
		}
	}
}