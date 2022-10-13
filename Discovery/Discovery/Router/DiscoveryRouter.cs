using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Discovery.Common;
using Discovery.Common.Data;
using Discovery.Storage;

namespace Discovery.Router
{
	public class DiscoveryRouter : IRouter
	{
		private const string Get = "GET";
		private const string Put = "PUT";
		
		private readonly IStorage _storage;
		
        public DiscoveryRouter(IStorage storage)
        {
            _storage = storage;
        }
        
		public void Route(HttpListenerRequest request, HttpListenerResponse response)
		{
			if (request.HttpMethod == Get)
            {
	            var endpoint = request.Url.AbsolutePath.TrimWeb();
	            var (ok, uri, param, endpoint0) = _storage.TryGet(endpoint);
	            if (ok == false)
	            {
		            HttpUtilities.NotFoundResponse(response);
		            return;
	            }
			
	            var uriData = new UriData(uri, param, endpoint0);
	            var jsonData = JsonSerializer.Serialize(uriData);
			
	            HttpUtilities.SendResponseMessage(response, jsonData);
            }
			else if (request.HttpMethod == Put)
			{
				var content = HttpUtilities.ReadRequestBody(request);
				UriData uriData;
				try
				{
					uriData = JsonSerializer.Deserialize<UriData>(content,
						new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				}
				catch (Exception)
				{
					response.StatusCode = 400;
					return;
				}

				if (string.IsNullOrEmpty(uriData.Uri) ||
				    string.IsNullOrEmpty(uriData.Endpoint))
				{
					response.StatusCode = 400;
					return;
				}
				
				_storage.TryMark(uriData.Uri, uriData.Endpoint);
			}
			else
			{
				HttpUtilities.NotFoundResponse(response);
				return;
			}
		}

		public void ResendData(HttpListenerRequest request, HttpListenerResponse response, string uri)
		{
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