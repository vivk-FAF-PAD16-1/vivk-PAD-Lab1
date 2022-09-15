using System.IO;
using System.Linq;
using System.Net;

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
    }
}