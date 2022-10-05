using System.Net;

namespace Auth.Router
{
    public interface IRouter
    {
        void Route(HttpListenerRequest request, HttpListenerResponse response);
    }
}