using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using NUnit.Framework;

namespace SelfishHttp.Test
{
    public class SelfishSocketTest
    {
        private Server _server;
        private string BaseUrl;


        [SetUp]
        public void SetUp()
        {
            var port = 12345;
            _server = new Server(port);
            BaseUrl = string.Format("http://localhost:{0}/", port);
        }

        [TearDown]
        public void TearDown()
        {
            _server.Stop();
        }

        [Test]
        public void ShouldReturnChangeOfProtocolResponse()
        {
            _server.OnSocket("/sockets").RespondWith("yes, this is stuff");

            var client = new HttpClient();
            var message = new HttpRequestMessage(new HttpMethod("Get"), "http://localhost:12345/sockets");
            message.Headers.Add("Host", "abc");
            message.Headers.Add("Upgrade", "websocket");
            message.Headers.Add("Connection", "upgrade");

            var response = client.SendAsync(message).Result;
            Assert.That(response.StatusCode, Is.EqualTo(101));
        }
    }
}
