using System;
using System.Net;
using News.Common;
using News.Counter;
using News.Endpoints;

namespace News.Router
{
    public class NewsRouter : IRouter
    {
        private readonly NewsEndpoints _endpoints;
        private readonly ICounter _counter;

        private enum Router
        {
            Default = 1,
            Id = 2,
        }
        
        public NewsRouter(ICounter counter)
        {
            _endpoints = new NewsEndpoints();
            _counter = counter;
        }
        
        public async void Route(HttpListenerRequest request, HttpListenerResponse response)
        {
            var segments = request.Url.Segments;
            var segmentsCount = segments.Length;

            const int defaultIndex = (int)Router.Default;
            const int idIndex = (int)Router.Id;

            var isFull = _counter.IsFull();
            if (isFull)
            {
                HttpUtilities.TooManyRequestsResponse(response);
                
                response.Close();
                return;
            }
            
            _counter.Register();
            
            switch (segmentsCount)
            {
                case defaultIndex:
                    await _endpoints.RouteAll(request, response);
                    break;
                case idIndex:
                    int id;
                    try
                    {
                        id = Convert.ToInt32(segments[idIndex - 1].TrimWeb());
                    }
                    catch (Exception e)
                    {
                        HttpUtilities.NotFoundResponse(response);
                        break;
                    }
                    await _endpoints.RouteById(request, response, id);
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