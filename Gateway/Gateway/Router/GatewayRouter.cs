using System;
using System.Net;
using Gateway.Common;
using Gateway.Storage;

namespace Gateway.Router
{
	public class GatewayRouter : IRouter
	{
		private IStorage _storage;
        
        private const string NotFoundMessage = "OLEG NOT FOUND!";
        private const int NotFoundStatus = 404;

        public GatewayRouter(IStorage storage)
        {
            _storage = storage;
        }
		public void Route(HttpListenerRequest request, HttpListenerResponse response)
		{
			var endpoint = request.Url.AbsolutePath.TrimWeb();
			var (ok, uri) = _storage.TryGet(endpoint);
			if (ok == false)
			{
				// TODO: NOT FOUND
				return;
			}
			
			Console.WriteLine(uri);
		}
	}
}