using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using News.Common;
using News.Counter;
using News.Discovery;
using News.Endpoints;
using News.Listener;
using News.Router;

namespace News
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

			var newsRouter = new NewsRouter(ref configurationData, counter) as IRouter;

			var newsListener = new AsyncListener(
				configurationData.NewsPrefixes,
				newsRouter);
			newsListener.Schedule();

			if (!Independent)
			{
				var registrator = new Registrator(
					configurationData.DiscoveryUri, 
					configurationData.Routes) as IRegistrator;
			
				var ok = await registrator.Register();
				if (!ok)
				{
					newsListener.Stop();
					return;
				}
			}

			Thread.Sleep(Timeout.InfiniteTimeSpan);
		}
	}
}