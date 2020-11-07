using System;
using MTCG.Server;
using NUnit.Framework;

namespace MTCG.Test
{
    [TestFixture]
    public class ResponseContextTest
    {
        [Test]
        public void Test_ToString_NoContentAndStatus()
        {
            // arrange
            var response = new ResponseContext
            {
                Status = HttpStatus.NotFound
            };
            
            string date = DateTime.Now.ToString("R");
            response.Headers["Date"] = date;

            string expectedResponse = 
                "HTTP/1.1 404 Not Found\r\n" + 
                "Date: " + date + "\r\n" + 
                "Content-Length: 0\r\n" +
                "\r\n";
            
            // act
            string responseStr = response.ToString();
            
            // assert
            Assert.AreEqual(expectedResponse, responseStr);
        }

        [Test]
        public void Test_ToString_CustomMessage()
        {
            // arrange
            var response = new ResponseContext
            {
                Content = "\"123456\"",
                Protocol = "HTTPS",
                Version = "2.0",
                Status = HttpStatus.Created,
                ContentType = MediaType.Json
            };
            
            string expectedDate = DateTime.Now.ToString("R");
            response.Headers["Date"] = expectedDate;
            
            string expectedResponse = 
                "HTTPS/2.0 201 Created\r\n" + 
                "Content-Type: application/json\r\n" +
                "Date: " + expectedDate + "\r\n" + 
                "Content-Length: 8\r\n" +
                "\r\n" + 
                "\"123456\"";
            
            // act
            string responseStr = response.ToString();

            // assert
            Assert.AreEqual(expectedResponse, responseStr);
        }
        
    }
}