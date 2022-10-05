using System.Net;
using Auth.Common;
using Auth.Common.Data;
using Auth.Counter;
using Auth.Endpoints;

namespace Auth.Router
{
	public class AuthRouter : IRouter
    {
        private const string RegisterEndpoint = "register";
        private const string LoginEndpoint = "login";

        private readonly AuthEndpoints _endpoint;
        private readonly ICounter _counter;

        public AuthRouter(ref ConfigurationData data, ICounter counter)
        {
            _endpoint = new AuthEndpoints(ref data);
            _counter = counter;
        }

        public async void Route(HttpListenerRequest request, HttpListenerResponse response)
        {
            var segments = request.Url.Segments;
            if (segments.Length != 2)
            {
                HttpUtilities.NotFoundResponse(response);
                return;
            }
            
            var isFull = _counter.IsFull();
            if (isFull)
            {
                HttpUtilities.TooManyRequestsResponse(response);
                return;
            }
            
            _counter.Register();
            
            var endpoint = segments[1].TrimWeb();

            switch (endpoint)
            {
                case LoginEndpoint:
                    await _endpoint.RouteLogin(request, response);
                    break;
                case RegisterEndpoint:
                    await _endpoint.RouteRegister(request, response);
                    break;
                default:
                    HttpUtilities.NotFoundResponse(response);
                    break;
            }

            _counter.Unregister();
            
            response.Close();
        }
    }
}