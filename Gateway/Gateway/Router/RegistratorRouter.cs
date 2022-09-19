using System.Net;
using System.Text.Json;
using Gateway.Common;
using Gateway.Common.Data;
using Gateway.Storage;

namespace Gateway.Router
{
    public class RegistratorRouter : IRouter
    {
        private IStorage _storage;

        private const string Post = "POST";

        public RegistratorRouter(IStorage storage)
        {
            _storage = storage;
        }
        
        public void Route(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod != Post)
            {
                HttpUtilities.NotFoundResponse(response);
                return;
            }

            if (request.Url.Segments.Length != 1)
            {
                HttpUtilities.NotFoundResponse(response);
                return; 
            }

            if (request.HasEntityBody == false)
            {
                HttpUtilities.NotFoundResponse(response);
                return; 
            }

            var json = HttpUtilities.ReadRequestBody(request);
            if (JsonUtilities.IsValid(json) == false)
            {
                HttpUtilities.NotFoundResponse(response);
                return;  
            }
            
            var route = JsonSerializer.Deserialize<RouteData>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (route.IsValid == false)
            {
                HttpUtilities.NotFoundResponse(response);
                return;
            }
            
            _storage.Register(route.Endpoint, route.DestinationUri);
        }
    }
}