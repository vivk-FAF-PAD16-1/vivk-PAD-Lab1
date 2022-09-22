using System.Net;

namespace News.Router
{
    public interface IRouter
    {
        void Route(HttpListenerRequest request, HttpListenerResponse response);
    }
}