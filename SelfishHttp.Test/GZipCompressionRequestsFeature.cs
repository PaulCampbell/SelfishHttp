using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using NUnit.Framework;

namespace SelfishHttp.Test
{
    [TestFixture]
    public class GZipCompressionRequestsFeature : SelfishHttpFeature
    {
        [Test]
        public void CanRespondWithGzippedString()
        {
            const string stringToZipUp = "Here's some stuff that'll get gzipped up";
            _server.OnGet("/gzipped").AllowGZipCompressionRequests().Respond((req,res) => { res.Body = stringToZipUp; });
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:12345/gzipped");
            request.Headers.Add("Accept-Encoding", "gzip");

            var response = client.SendAsync(request).Result;
           
            Assert.That(response.Content.Headers.ContentEncoding.First(), Is.EqualTo("gzip"));
            Assert.That(UnzipString(response.Content.ReadAsByteArrayAsync().Result), Is.EqualTo(stringToZipUp));

        }


        [Test]
        public void NonGZipRequestsDoNotGetZipped()
        {
            const string unzippedResponse = "Here's some stuff that wont get zipped";
            _server.OnGet("/not-zipped").AllowGZipCompressionRequests().Respond((req,res)=>
                                                                                    {
                                                                                        res.Body = unzippedResponse;
                                                                                    });
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:12345/not-zipped");
            
            var response = client.SendAsync(request).Result;

            Assert.That(response.Content.Headers.ContentEncoding.Any(x => x == "gzip"), Is.False);
            Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo(unzippedResponse));
        }


        private string UnzipString(byte[] zipped)
        {
            var sampleOut = "";
            using (var decomStream = new MemoryStream(zipped))
            using (var hgs = new GZipStream(decomStream, CompressionMode.Decompress))
            {
                using (var reader = new StreamReader(hgs))
                {
                    sampleOut = reader.ReadToEnd();
                }
            }

            return sampleOut;
        }
    }
}