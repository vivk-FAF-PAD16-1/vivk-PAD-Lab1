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

        public NewsEndpoints()
        {
            _model = new NewsModel("localhost", 3306, "root", "root");
        }
        
        public async Task RouteAll(HttpListenerRequest request, HttpListenerResponse response)
        {
            var method = request.HttpMethod;

            switch (method)
            {
                case Get:
                    // TODO: get all news
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

        public void RouteById(HttpListenerRequest request, HttpListenerResponse response, string id)
        {
            var method = request.HttpMethod;

            switch (method)
            {
                case Get:
                    // TODO: get news item by id
                    break;
                case Put:
                    // TODO: update news item by id
                    break;
                default:
                    HttpUtilities.NotFoundResponse(response);
                    break;
            }
        }
    }
}