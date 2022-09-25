using System.Net;
using News.Common;

namespace News.Endpoints
{
    public class NewsEndpoints
    {
        private const string Get = "GET";
        private const string Post = "POST";
        private const string Put = "PUT";

        public NewsEndpoints()
        {
            
        }
        
        public void RouteAll(HttpListenerRequest request, HttpListenerResponse response)
        {
            var method = request.HttpMethod;

            switch (method)
            {
                case Get:
                    // TODO: get all news
                    break;
                case Post:
                    // TODO: create new news feed
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