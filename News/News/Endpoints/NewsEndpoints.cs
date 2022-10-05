using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using News.Common;
using News.Common.Data;
using News.Model;

namespace News.Endpoints
{
    public class NewsEndpoints
    {
        private const string Get = "GET";
        private const string Post = "POST";
        private const string Put = "PUT";

        private const int TimeoutMillisecondsDelay = 1000;

        private readonly NewsModel _model;

        public NewsEndpoints(ref ConfigurationData conf)
        {
            _model = new NewsModel(
                conf.ServerDb,
                conf.PortDb,
                conf.UserDb,
                conf.PassDb);
        }
        
        public async Task RouteAll(HttpListenerRequest request, HttpListenerResponse response)
        {
            var method = request.HttpMethod;
            var isTimeout = false;

            switch (method)
            {
                
                case Get:
                    const int number = 20;
                    
                    var dest = new List<NewsData>(capacity: number);
                    isTimeout = await HttpUtilities
                        .Timeout(_model.Get(number, dest), TimeoutMillisecondsDelay);

                    if (isTimeout)
                    {
                        HttpUtilities.RequestTimeoutResponse(response);
                        break;
                    }
                    
                    var json = JsonSerializer.Serialize(dest,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    HttpUtilities.SendResponseMessage(response, json);
                    break;
                case Post:
                    var body = HttpUtilities.ReadRequestBody(request);
                    var isValid = JsonUtilities.IsValid(body);
                    if (!isValid)
                    {
                        HttpUtilities.BadRequestResponse(response);
                        return;
                    }

                    var data = JsonSerializer.Deserialize<NewsData>(body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    isTimeout = await HttpUtilities
                        .Timeout(_model.Create(data), TimeoutMillisecondsDelay);

                    if (isTimeout)
                    {
                        HttpUtilities.RequestTimeoutResponse(response);
                        break;
                    }
                    
                    break;
                default:
                    HttpUtilities.NotFoundResponse(response);
                    break;
            }
        }

        public async Task RouteById(HttpListenerRequest request, HttpListenerResponse response, int id)
        {
            var method = request.HttpMethod;

            switch (method)
            {
                case Get:
                    var (result0, isTimeout0) = await HttpUtilities
                        .Timeout(_model.Get(id), TimeoutMillisecondsDelay);
                    if (isTimeout0)
                    {
                        HttpUtilities.RequestTimeoutResponse(response);
                        break;
                    }
                    
                    var (dataGet, isFound) = result0;
                    if (!isFound)
                    {
                        HttpUtilities.NotFoundResponse(response);
                        break;
                    }
                    
                    var json = JsonSerializer.Serialize(dataGet,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    HttpUtilities.SendResponseMessage(response, json);
                    break;
                case Put:
                    var body = HttpUtilities.ReadRequestBody(request);
                    var isValid = JsonUtilities.IsValid(body);
                    if (!isValid)
                    {
                        HttpUtilities.BadRequestResponse(response);
                        break;
                    }

                    var dataPut = JsonSerializer.Deserialize<NewsData>(body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    dataPut.Id = id;
                    
                    var (resultPut, isTimeoutPut) = await HttpUtilities
                        .Timeout(_model.Update(dataPut), TimeoutMillisecondsDelay);
                    if (isTimeoutPut)
                    {
                        HttpUtilities.RequestTimeoutResponse(response);
                        break;
                    }
                    
                    if (!resultPut)
                    {
                        HttpUtilities.NotFoundResponse(response);
                        break;
                    }
                    break;
                default:
                    HttpUtilities.NotFoundResponse(response);
                    break;
            }
        }
    }
}