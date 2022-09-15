using System;
using System.Net;
using Gateway.Common;
using Gateway.Storage;

namespace Gateway.Router
{
	public class GatewayRouter : IRouter
	{
		private IStorage _storage;
		
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
				HttpUtilities.NotFoundResponse(response);
				return;
			}
			
			Console.WriteLine(uri);
		}
	}
}