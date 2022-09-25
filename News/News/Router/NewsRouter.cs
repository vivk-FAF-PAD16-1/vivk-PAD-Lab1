using System.Net;
using News.Common;
using News.Endpoints;

namespace News.Router
{
    public class NewsRouter : IRouter
    {
        private NewsEndpoints _endpoints;

        private enum Router
        {
            Default = 1,
            Id = 2,
        }
        
        public NewsRouter()
        {
            _endpoints = new NewsEndpoints();
        }
        
        public void Route(HttpListenerRequest request, HttpListenerResponse response)
        {
            var segments = request.Url.Segments;
            var segmentsCount = segments.Length;

            const int defaultIndex = (int)Router.Default;
            const int idIndex = (int)Router.Id;
            
            switch (segmentsCount)
            {
                case defaultIndex:
                    _endpoints.RouteAll(request, response);
                    break;
                case idIndex:
                    var id = segments[idIndex - 1].TrimWeb();
                    _endpoints.RouteById(request, response, id);
                    break;
                default:
                    HttpUtilities.NotFoundResponse(response);
                    break;
            }
            
            response.Close();
        }
    }
}