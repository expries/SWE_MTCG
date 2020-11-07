using System.Collections.Generic;
using MTCG.Server;
using NUnit.Framework;

namespace MTCG.Test
{
    [TestFixture]
    public class RequestContextTest
    {
        [Test]
        public void Test_Read_HttpGet()
        {
            // arrange
            const string rawRequest = 
                "GET /messages/123456/ HTTP/1.1\r\n" +
                "User-Agent: MyClient/1.0.0\r\n" +
                "Accept: */*\r\n" +
                "Accept-Encoding: gzip, deflate, br\r\n" +
                "Connection: keep-alive\r\n";

            const string method = "GET";
            const string protocol = "HTTP";
            const string version = "1.1";
            const string path = "/messages/123456";
            
            var expectedHeader = new Dictionary<string, string>
            {
                { "User-Agent", "MyClient/1.0.0" },
                { "Accept", "*/*" },
                { "Accept-Encoding", "gzip, deflate, br" },
                { "Connection", "keep-alive" }
            };

            // act
            var request = new RequestContext();
            request.Read(rawRequest);
            
            // assert
            Assert.AreEqual(method, request.Method);
            Assert.AreEqual(protocol, request.Protocol);
            Assert.AreEqual(version, request.Version);
            Assert.AreEqual(path, request.Path);
            Assert.AreEqual(null, request.Content);
            CollectionAssert.AreEqual(expectedHeader, request.Headers);
        }

        [Test]
        public void Test_Read_HttpPost()
        {
            // arrange
            const string rawRequest = 
                "POST /messages/ HTTP/1.0\r\n" +
                "Content-Type: text/plain\r\n" +
                "User-Agent: Mozilla/5.0 (Windows NT 10.0; rv:78.0) Firefox/78.0\r\n" +
                "Accept: */*\r\n" +
                "Accept-Encoding: gzip, deflate, br\r\n" +
                "Connection: keep-alive\r\n" +
                "Content-Length: 25\r\n" + 
                "\r\n" + 
                "My message is\r\n" + 
                "multiline!";
            
            const string method = "POST";
            const string protocol = "HTTP";
            const string version = "1.0";
            const string path = "/messages";
            const string content = "My message is\r\nmultiline!";
            
            var expectedHeader = new Dictionary<string, string>
            {
                { "Content-Type", "text/plain" },
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; rv:78.0) Firefox/78.0" },
                { "Accept", "*/*" },
                { "Accept-Encoding", "gzip, deflate, br" },
                { "Connection", "keep-alive" },
                { "Content-Length", "25" }
            };
            
            // act
            var request = new RequestContext();
            request.Read(rawRequest);

            // assert
            Assert.AreEqual(method, request.Method);
            Assert.AreEqual(protocol, request.Protocol);
            Assert.AreEqual(version, request.Version);
            Assert.AreEqual(path, request.Path);
            Assert.AreEqual(content, request.Content);
            CollectionAssert.AreEqual(expectedHeader, request.Headers);
        }
    }
}