using System.Linq;
using System.Net.Http;
using NUnit.Framework;

namespace SelfishHttp.Test
{
    [TestFixture]
    public class GZipCompressionRequestsFeature : SelfishHttpFeature
    {
        [Test]
        public void CanRespondWithGzippedContent()
        {
            _server.OnGet("/gzipped").RespondWith("Here's some stuff that'll get gzipped up").AllowGZipCompressionRequests();
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:12345/gzipped");
            request.Headers.Add("Accept-Encoding", "gzip");

            var response = client.SendAsync(request).Result;
           
            Assert.That(response.Content.Headers.ContentEncoding.First(), Is.EqualTo("gzip"));
            Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo("zipped"));

        }


        [Test]
        public void NonGZipRequestsDoNotGetZipped()
        {
            const string unzippedResponse = "Here's some stuff that wont get zipped";
            _server.OnGet("/not-zipped").RespondWith(unzippedResponse).AllowGZipCompressionRequests();
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:12345/not-zipped");
            
            var response = client.SendAsync(request).Result;

            Assert.That(response.Content.Headers.ContentEncoding.Any(x => x == "gzip"), Is.False);
            Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo(unzippedResponse));
        }
    }
}