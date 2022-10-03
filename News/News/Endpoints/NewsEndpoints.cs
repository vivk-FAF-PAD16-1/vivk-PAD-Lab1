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

            switch (method)
            {
                case Get:
                    const int number = 20;
                    
                    var dest = new List<NewsData>(capacity: number);
                    await _model.Get(number, dest);

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

                    await _model.Create(data);
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
                    var (dataGet, isFound) = await _model.Get(id);
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
                    
                    var ok = await _model.Update(dataPut);
                    if (!ok)
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