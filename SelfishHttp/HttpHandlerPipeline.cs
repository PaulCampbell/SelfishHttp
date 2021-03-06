using System;
using System.Collections.Generic;
using System.Net;

namespace SelfishHttp
{
    public class HttpHandler : IHttpHandler
    {
        private IList<Action<HttpListenerContext, Action>> _handlers;

        public HttpHandler(IServerConfiguration serverConfig)
        {
            _handlers = new List<Action<HttpListenerContext, Action>>();
            ServerConfiguration = serverConfig;
        }

        public void Handle(HttpListenerContext context, Action next)
        {
            var handlerEnumerator = _handlers.GetEnumerator();
            Action handle = null;
            handle = () =>
                         {
                             if (handlerEnumerator.MoveNext())
                             {
                                 handlerEnumerator.Current(context, () => handle());
                             } else
                             {
                                 next();
                             }
                         };

            handle();
        }

        public AuthenticationSchemes? AuthenticationScheme { get; set; }
        public IServerConfiguration ServerConfiguration { get; private set; }

        public void AddHandler(Action<HttpListenerContext, Action> handler)
        {
            _handlers.Add(handler);
        }
    }
}