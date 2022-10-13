using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gateway.Common;
using Gateway.Common.Data;

namespace Gateway.Router
{
	public class GatewayRouter : IRouter
	{
		private readonly string _discoveryUri;

		private readonly HttpClient _httpClient;

		private const string Get = "GET";
		private const string Put = "PUT";
		
        public GatewayRouter(string discoveryUri)
        {
	        _discoveryUri = discoveryUri.TrimWeb();

	        _httpClient = new HttpClient();
        }
        
		public async void Route(HttpListenerRequest request, HttpListenerResponse response)
		{
			await RouteInternal(request, response);
			response.Close();
		}

		private async Task RouteInternal(HttpListenerRequest request, HttpListenerResponse response)
		{
			var absolutePath = request.Url.AbsolutePath;
			var discoveryFullUri = _discoveryUri + absolutePath;

			HttpRequestMessage discoveryRequest = null;
			HttpResponseMessage discoveryResponse = null;
			HttpResponseMessage serviceResponse = null;

			bool success;

			do
			{
				discoveryRequest = new HttpRequestMessage(new HttpMethod(Get), discoveryFullUri);
				
				try
				{
					discoveryResponse = await _httpClient.SendAsync(discoveryRequest);
				}
				catch (Exception)
				{
					HttpUtilities.NotFoundResponse(response);
					return;	
				}

				var discoveryContent = HttpUtilities.ReadResponseBody(discoveryResponse);
				if (discoveryResponse.IsSuccessStatusCode == false)
				{
					HttpUtilities.NotFoundResponse(response);
					return;
				}

				var uriData = JsonSerializer.Deserialize<UriData>(discoveryContent,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				var content = HttpUtilities.ReadRequestBody(request);

				var serviceUri = uriData.Uri + uriData.Param;
				var serviceRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), serviceUri);
			
				if (request.HttpMethod != Get && content != string.Empty)
				{
					serviceRequest.Content = new StringContent(content, Encoding.UTF8, request.ContentType);
				}

				success = true;
				
				try
				{
					serviceResponse = await _httpClient.SendAsync(serviceRequest);
				}
				catch (Exception)
				{
					discoveryRequest = new HttpRequestMessage(new HttpMethod(Put), discoveryFullUri);
					content = JsonSerializer.Serialize(uriData,
						new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
					discoveryRequest.Content = new StringContent(content, Encoding.UTF8);
					
					try
					{
						await _httpClient.SendAsync(discoveryRequest);
					}
					catch (Exception)
					{
						HttpUtilities.NotFoundResponse(response);
						return;	
					}
	
					success = false;
				}
			} while (!success);

			var serviceContent = HttpUtilities.ReadResponseBody(serviceResponse);
			var serviceStatusCode = (int)serviceResponse.StatusCode;
			if (serviceContent != null)
			{
				HttpUtilities.SendResponseMessage(response, serviceContent, serviceStatusCode);
			}
			else
			{
				HttpUtilities.SendResponse(response, serviceStatusCode);
			}
		}
	}
}