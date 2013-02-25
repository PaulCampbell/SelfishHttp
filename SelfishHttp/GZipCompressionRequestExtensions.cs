using System.Linq;

namespace SelfishHttp
{
    public static class GZipCompressionRequestExtensions
    {
        public static T AllowGZipCompressionRequests<T>(this T handler) where T : IHttpHandler
        {
            return handler.Respond((req, res, next) =>
                                       {
                                           if (!string.IsNullOrEmpty(req.Headers.Get("Accept-Encoding")))
                                           {
                                               if (req.Headers.Get("Accept-Encoding").Contains("gzip"))
                                               {
                                                   res.Headers.Add("Content-Encoding", req.Headers.Get("Accept-Encoding"));
                                                   res.Body = ZipUpResponseBody(res.Body);
                                               }
                                           }
                                           next();
                                       });
        }

        private static object ZipUpResponseBody(object body)
        {
            return "compressed string";
        }
    }
}