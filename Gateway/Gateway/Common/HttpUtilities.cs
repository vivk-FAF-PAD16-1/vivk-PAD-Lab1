using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Common
{
    public static class HttpUtilities
    {
        public static string ReadRequestBody(HttpListenerRequest request)
        {
            var body = request.InputStream;
            var encoding = request.ContentEncoding;

            var reader = new StreamReader(body, encoding);
            var content = reader.ReadToEnd();
            
            reader.Close();
            body.Close();

            return content;
        }
        
        public static string ReadResponseBody(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public static void SendResponseMessage(HttpListenerResponse response, string message, int statusCode = 200)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = statusCode;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
        
        private const string NotFoundMessage = "OLEG NOT FOUND!";
        private const int NotFoundStatus = 404;
        
        public static void NotFoundResponse(HttpListenerResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes(NotFoundMessage);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = NotFoundStatus;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
        
        private const string TooManyRequestsMessage = "429 Too Many Requests";
        private const int TooManyRequestsStatus = 429;
        
        public static void TooManyRequestsResponse(HttpListenerResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes(TooManyRequestsMessage);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = TooManyRequestsStatus;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private const string BadRequestMessage = "400 Bad Request";
        private const int BadRequestStatusCode = 400;
        
        public static void BadRequestResponse(HttpListenerResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes(BadRequestMessage);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = BadRequestStatusCode;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
        
        private const string RequestTimeoutMessage = "408 Request Timeout";
        private const int RequestTimeoutStatusCode = 408;
        
        public static void RequestTimeoutResponse(HttpListenerResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes(RequestTimeoutMessage);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = RequestTimeoutStatusCode;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private static readonly char[] WebCharacters = { '/', '\\' };

        public static string TrimWeb(this string source)
        {
            return source.Trim(WebCharacters);
        }

        public static bool StartWith(this string source, string find)
        {
            var length = find.Length;
            if (length > source.Length)
            {
                return false;
            }
            
            for (var i = 0; i < length; i++)
            {
                if (find[i] != source[i])
                {
                    return false;
                }
            }

            if (length != source.Length)
            {
                var webCharacter = source[length];
                if (WebCharacters.Contains(webCharacter) == false)
                {
                    return false;
                }
            }

            return true;
        }
        
        public static async Task<(TResult, bool)> Timeout<TResult>(
            Task<TResult> task, 
            int millisecondsDelay) 
        {
            var waitDelay = Task.Delay(millisecondsDelay);
            await Task.WhenAny(waitDelay, task);

            return task.IsCompleted 
                ? (task.Result, false) 
                : (default, true);
        }

        public static async Task<bool> Timeout(
            Task task,
            int millisecondsDelay)
        {
            var waitDelay = Task.Delay(millisecondsDelay);
            await Task.WhenAny(waitDelay, task);

            return !task.IsCompleted;
        }
        
        public static bool Ping(string uri, int timeout = 1000)
        {
            var pingable = false;
            var pinger = new Ping();

            try
            {
                var reply = pinger.Send(uri, timeout);
                if (reply != null)
                {
                    pingable = reply.Status == IPStatus.Success;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                pinger.Dispose();
            }

            return pingable;
        }
    }
}