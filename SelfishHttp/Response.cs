using System.Net;

namespace SelfishHttp
{
    public class Response : IResponse
    {
        private readonly IBodyWriter _bodyWriter;
        private readonly HttpListenerResponse _response;

        private ResponseEncoding ResponseEncoding
        {
            get
            {
                if (string.IsNullOrEmpty(Headers.Get("Content-Encoding"))) return ResponseEncoding.PlainText;
                if (Headers.Get("Content-Encoding").Contains("gzip")) return ResponseEncoding.GZip;
                return Headers.Get("Content-Encoding").Contains("deflate") ? ResponseEncoding.Deflate : ResponseEncoding.PlainText;
            }
        }

        public Response(IServerConfiguration config, HttpListenerResponse response)
        {
            _bodyWriter = config.BodyWriter;
            _response = response;
        }

        public int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return _response.Headers; }
        }

        public object Body
        {
            set { _bodyWriter.WriteBody(value ?? "", _response.OutputStream, ResponseEncoding); }
        }
    }
}