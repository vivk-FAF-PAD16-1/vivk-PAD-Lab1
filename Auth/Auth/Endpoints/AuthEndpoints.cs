using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Auth.Common;
using Auth.Common.Data;
using Auth.Model;

namespace Auth.Endpoints
{
	public class AuthEndpoints
	{
        private const string Post = "POST";

        private const string DatabaseName = "Auth";
        private const string CollectionName = "Users";

        private const string Secret = "letmein";
        
        private const int DbErrorStatusCode = 403;

        private const int TimeoutMillisecondsDelay = 1000;

        private readonly AuthModel _model;

        public AuthEndpoints(ref ConfigurationData conf)
        {
            _model = new AuthModel(
                conf.ConnectionDb,
                DatabaseName,
                CollectionName);
        }
        
        public async Task RouteLogin(HttpListenerRequest request, HttpListenerResponse response)
        {
            var method = request.HttpMethod;
            if (method != Post)
            {
                HttpUtilities.NotFoundResponse(response);
                return;
            }

            var body = HttpUtilities.ReadRequestBody(request);
            var isJson = JsonUtilities.IsValid(body);
            if (!isJson)
            {
                HttpUtilities.BadRequestResponse(response);
                return;
            }

            var authData = JsonSerializer.Deserialize<AuthData>(body, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (authData == null)
            {
                HttpUtilities.BadRequestResponse(response);
                return;
            }

            if (authData.IsValid())
            {
                HttpUtilities.BadRequestResponse(response);
                return;
            }

            var (error, isTimeout) = await HttpUtilities.Timeout(
                _model.Get(authData), 
                TimeoutMillisecondsDelay);

            if (isTimeout)
            {
                HttpUtilities.RequestTimeoutResponse(response);
                return;
            }
            
            if (error != null)
            {
                HttpUtilities.SendResponseMessage(
                    response, 
                    error, 
                    DbErrorStatusCode);
                return;
            }

            var jwtToken = JwtUtilities.Generate(authData.Email, Secret);
            HttpUtilities.SendResponseMessage(response, jwtToken);
        }
        
        public async Task RouteRegister(HttpListenerRequest request, HttpListenerResponse response)
        {
            var method = request.HttpMethod;
            if (method != Post)
            {
                HttpUtilities.NotFoundResponse(response);
                return;
            }

            var body = HttpUtilities.ReadRequestBody(request);
            var isJson = JsonUtilities.IsValid(body);
            if (!isJson)
            {
                HttpUtilities.BadRequestResponse(response);
                return;
            }

            var authData = JsonSerializer.Deserialize<AuthData>(body, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (authData == null)
            {
                HttpUtilities.BadRequestResponse(response);
                return;
            }

            if (authData.IsValid())
            {
                HttpUtilities.BadRequestResponse(response);
                return;
            }
            
            var (error, isTimeout) = await HttpUtilities.Timeout(
                _model.Create(authData), 
                TimeoutMillisecondsDelay);

            if (isTimeout)
            {
                HttpUtilities.RequestTimeoutResponse(response);
                return;
            }
            
            if (error != null)
            {
                HttpUtilities.SendResponseMessage(
                    response,
                    error,
                    DbErrorStatusCode);
                return;
            }
            
            var jwtToken = JwtUtilities.Generate(authData.Email, Secret);
            HttpUtilities.SendResponseMessage(response, jwtToken);
        }
    }	
}