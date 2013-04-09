
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
                                                  res.Headers.Add("Content-Encoding", req.Headers.Get("Accept-Encoding"));
                                           }
                                           next();
                                       });
        }

    }
}