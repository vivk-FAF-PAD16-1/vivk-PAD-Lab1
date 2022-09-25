using System;
using System.IO;
using System.Threading;
using News.Common;
using News.Counter;
using News.Listener;
using News.Router;

namespace News
{
	internal static class Program
	{
		private const string ConfigurationPath = "../../Resources/configuration.json";		
		
		public static void Main(string[] args)
		{
			var directoryAbsolutePath = AppDomain.CurrentDomain.BaseDirectory;
			var configurationAbsolutePath = Path.Combine(
				directoryAbsolutePath, ConfigurationPath);
			
			var configurator = new Configurator(configurationAbsolutePath);
			var configurationData = configurator.Load();

			var counter = new SyncCounter(configurationData.MaxCount) as ICounter;

			var newsRouter = new NewsRouter(counter) as IRouter;

			var newsListener = new AsyncListener(
				configurationData.NewsPrefixes,
				newsRouter);
			newsListener.Schedule();

			Thread.Sleep(Timeout.InfiniteTimeSpan);
		}
	}
}