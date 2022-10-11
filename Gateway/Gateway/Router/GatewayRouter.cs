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
			
			var discoveryRequest = new HttpRequestMessage(new HttpMethod(Get), discoveryFullUri);
			HttpResponseMessage discoveryResponse;

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

			var serviceRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), uriData.Uri);
			
			if (request.HttpMethod != Get && content != string.Empty)
			{
				serviceRequest.Content = new StringContent(content, Encoding.UTF8, request.ContentType);
			}
			
			HttpResponseMessage serviceResponse;

			try
			{
				serviceResponse = await _httpClient.SendAsync(serviceRequest);
			}
			catch (Exception)
			{
				HttpUtilities.NotFoundResponse(response);
				return;	
			}
			
			var serviceContent = HttpUtilities.ReadResponseBody(serviceResponse);
			var serviceStatusCode = (int)serviceResponse.StatusCode;
			if (serviceContent != null)
			{
				HttpUtilities.SendResponseMessage(response, serviceContent, serviceStatusCode);
			}
			else
			{
				response.StatusCode = serviceStatusCode;
			}
		}
	}
}