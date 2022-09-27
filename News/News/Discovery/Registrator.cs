using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using News.Common.Data;

namespace News.Discovery
{
	public class Registrator : IRegistrator
	{
		private const string Post = "POST";
		
		private readonly HttpClient _httpClient;
		private readonly string _discoveryUri;
		private readonly RouteData[] _routes;
		
		public Registrator(string discoveryUri, RouteData[] routes)
		{
			_httpClient = new HttpClient();
			_discoveryUri = discoveryUri;
			_routes = routes;
		}

		public async Task<bool> Register()
		{
			for (var i = 0; i < _routes.Length; i++)
			{
				var routeJson = JsonSerializer.Serialize<RouteData>(_routes[i],
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				var registratorRequest = new HttpRequestMessage(new HttpMethod(Post), _discoveryUri);
				registratorRequest.Content = new StringContent(routeJson, Encoding.UTF8);
				
				HttpResponseMessage registratorResponse;
				try
				{
					registratorResponse = await _httpClient.SendAsync(registratorRequest);
				}
				catch (Exception e)
				{
					Console.WriteLine($"Registrator ERROR: {e}");
					return false;
				}
				
				if (!registratorResponse.IsSuccessStatusCode)
				{
					Console.WriteLine($"Registrator ERROR: Status Code {registratorResponse.StatusCode}");
					return false;
				}
			}

			return true;
		}
	}
}