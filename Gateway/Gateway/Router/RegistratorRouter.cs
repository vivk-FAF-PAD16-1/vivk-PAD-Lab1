using System;
using System.Net;
using System.Text;
using System.Text.Json;
using Gateway.Common;

namespace Gateway.Router
{
    public class RegistratorRouter : IRouter
    {
        private const string NotFoundMessage = "OLEG NOT FOUND!";
        private const int NotFoundStatus = 404;
        
        private const string Post = "POST";
        
        public void Route(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod != Post)
            {
                NotFound(response);
                return;
            }

            if (request.Url.Segments.Length != 1)
            {
                NotFound(response);
                return; 
            }

            if (request.HasEntityBody == false)
            {
                NotFound(response);
                return; 
            }

            var json = HttpUtilities.ReadRequestBody(request);
            if (JsonUtilities.IsValid(json) == false)
            {
                NotFound(response);
                return;  
            }
            
            var route = JsonSerializer.Deserialize<RouteData>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (route.IsValid == false)
            {
                NotFound(response);
                return;
            }
            
            // TODO: WRITE IN ROUTES STORAGE
            Console.WriteLine($"{route.Endpoint} {route.DestinationUri}");
        }

        private void NotFound(HttpListenerResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes(NotFoundMessage);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = NotFoundStatus;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}