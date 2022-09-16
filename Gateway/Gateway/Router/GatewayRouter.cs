using System.Net;
using System.Net.Http;
using System.Text;
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

			var requestContent = HttpUtilities.ReadRequestBody(request);

			var client = new HttpClient();
			var newRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), uri);
			newRequest.Content = new StringContent(requestContent, Encoding.UTF8, request.ContentType);
			
			//TODO: Send request from circuit breaker

			var newResponse = client.SendAsync(newRequest)
				.GetAwaiter()
				.GetResult();
			var body = HttpUtilities.ReadResponseBody(newResponse);
			
            var buffer = Encoding.UTF8.GetBytes(body);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = (int)newResponse.StatusCode;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
		}
	}
}