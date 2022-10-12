using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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

        private const string CacheDbName = "NewsDatabase";
        private const string CacheModelName = "NewsData";

        private const int TimeoutMillisecondsDelay = 1000;

        private readonly HttpClient _httpClient;
        private readonly NewsModel _model;

        private readonly string _cacheUri;
        private readonly bool _independent;

        public NewsEndpoints(ref ConfigurationData conf)
        {
            _model = new NewsModel(
                conf.ServerDb,
                conf.PortDb,
                conf.UserDb,
                conf.PassDb);
            
	        _httpClient = new HttpClient();
            var cacheUriDomain = conf.CacheUri.TrimWeb();
            _cacheUri = $"{cacheUriDomain}/{CacheDbName}/{CacheModelName}";
            _independent = conf.Independent;
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

                    if (!_independent)
                    {
                        var cacheRequest = new HttpRequestMessage(HttpMethod.Post, _cacheUri);
                        cacheRequest.Content = new StringContent(json, Encoding.UTF8, request.ContentType);
                        await _httpClient.SendAsync(cacheRequest);
                    }

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
                    NewsData newsData = default;
                    var hasAny = false;
                    
                    if (!_independent)
                    {
                        var cacheRequest = new HttpRequestMessage(HttpMethod.Get, _cacheUri);
                        var cacheResponse = await _httpClient.SendAsync(cacheRequest);
                        
                        var cacheContent = HttpUtilities.ReadResponseBody(cacheResponse);
                        var data = JsonSerializer.Deserialize<NewsData[]>(cacheContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (data != null)
                        {
                            for (var i = 0; i < data.Length; i++)
                            {
                                if (data[i].Id == id)
                                {
                                    newsData = data[i];
                                    hasAny = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!hasAny)
                    {
                        var (result0, isTimeout0) = await HttpUtilities
                            .Timeout(_model.Get(id), TimeoutMillisecondsDelay);
                        if (isTimeout0)
                        {
                            HttpUtilities.RequestTimeoutResponse(response);
                            break;
                        }

                        bool isFound;
                        (newsData, isFound) = result0;
                        if (!isFound)
                        {
                            HttpUtilities.NotFoundResponse(response);
                            break;
                        }
                    }
                    
                    
                    
                    var json = JsonSerializer.Serialize(newsData,
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