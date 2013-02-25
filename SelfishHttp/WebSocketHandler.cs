using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SelfishHttp
{
    public class WebSocketHandler : IHttpResourceHandler
    {
        public IServerConfiguration ServerConfiguration { get; private set; }
        private string Path;
        private HttpHandler _pipeline;

        public WebSocketHandler(string path, IServerConfiguration serverConfiguration)
        {
            Path = path;
            ServerConfiguration = serverConfiguration;
            AuthenticationScheme = AuthenticationSchemes.Anonymous;
            _pipeline = new HttpHandler(serverConfiguration);
        }

        public void AddHandler(Action<HttpListenerContext, Action> handler)
        {
            _pipeline.AddHandler(handler);
        }

        public void Handle(HttpListenerContext context, Action next)
        {
            _pipeline.Handle(context, next);
        }

        public AuthenticationSchemes? AuthenticationScheme { get; set; }

        public bool Matches(HttpListenerRequest request)
        {
            return request.HttpMethod == "GET" && request.Url.AbsolutePath == Path
                && request.Headers["Upgrade"] == "websocket" &&  request.Headers["Connection"] == "upgrade";
        }
    }
}