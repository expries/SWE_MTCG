using System;
using Moq;
using MTCG.Exceptions;
using MTCG.Server;
using MTCG.Server.TcpWrapper;
using NUnit.Framework;

namespace MTCG.Test
{
    [TestFixture]
    public class WebServerTest
    {
        private Mock<ITcpClient> _client;
        private WebServer _server;

        [SetUp]
        public void CreateMocksAndServer()
        {
            _client = new Mock<ITcpClient>();
            _server = new WebServer();
        }

        [SetUp]
        public void SetupMockMethods()
        {
            _client.Setup(client => client.GetRequest())
                .Returns(() => new RequestContext());
            
            _client.Setup(client => client.Close()).Callback(() => {});
        }

        [Test]
        public void Test_ServeClient_ParameterlessRouteInvoked()
        {
            // arrange
            var request = new RequestContext
            {
                Method = "GET",
                Path = "/resource"
            };
            var response = new ResponseContext
            {
                Status = HttpStatus.Ok,
                ContentType = MediaType.Json
            };

            // act
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.RegisterRoute("GET", "/resource", _ => response);
            _server.ServeClient(_client.Object);

            // assert
            _client.Verify(client => client.SendResponse(response), Times.Once);
        }
        
        [Test]
        public void Test_ServeClient_ParameterRouteInvoked()
        {
            // arrange
            const string resourceId = "111";
            var request = new RequestContext
            {
                Method = "GET",
                Path = $"/resource/{resourceId}"
            };

            // act
            _server.RegisterRoute("GET", "/resource/{id}", context =>
            {
                return new ResponseContext
                {
                    Content = context.PathParam["id"]
                };
            });
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Content.Equals(resourceId))));
        }
        
        [Test]
        public void Test_ServeClient_MultiParameterRouteInvoked()
        {
            // arrange
            const string resourceId = "111";
            const string subResourceId = "222";
            const string responseContent = resourceId + subResourceId;
            
            var request = new RequestContext
            {
                Method = "GET",
                Path = $"/resource/{resourceId}/subresource/{subResourceId}"
            };

            // act
            _server.RegisterRoute("GET", "/resource/{id}/subresource/{sub-id}", context =>
            {
                return new ResponseContext
                {
                    Content = context.PathParam["id"] + context.PathParam["sub-id"]
                };
            });
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Content.Equals(responseContent))));
        }
        
        [Test]
        public void Test_ServeClient_EndpointNotFound()
        {
            // arrange
            const HttpStatus status = HttpStatus.NotFound;
            var request = new RequestContext
            {
                Method = "GET",
                Path = "/non/existent/route"
            };

            // act
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }

        [Test]
        public void Test_ServeClient_HttpVerbNotSupportedForRoute()
        {
            // arrange
            const HttpStatus status = HttpStatus.MethodNotAllowed;
            var request = new RequestContext
            {
                Method = "TRACE",
                Path = "/resource"
            };

            // act
            _server.RegisterRoute("GET", "/resource", _ => null);
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_ServeClient_ResourceNotFound()
        {
            // arrange
            const HttpStatus status = HttpStatus.NotFound;
            var request = new RequestContext
            {
                Method = "GET",
                Path = "/resource"
            };

            // act
            _server.RegisterRoute("GET", "/resource", _ => throw new NotFoundException());
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_ServeClient_BadFormatException()
        {
            // arrange
            const HttpStatus status = HttpStatus.BadRequest;
            var request = new RequestContext
            {
                Method = "GET",
                Path = "/resource"
            };

            // act
            _server.RegisterRoute("GET", "/resource", _ => throw new BadRequestException());
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_ServeClient_InternalServerErrorException()
        {
            // arrange
            const HttpStatus status = HttpStatus.InternalServerError;
            var request = new RequestContext
            {
                Method = "GET",
                Path = "/resource"
            };

            // act
            _server.RegisterRoute("GET", "/resource", _ => throw new Exception());
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_ServeClient_MultipleMethodsOnSameEndpoint()
        {
            // arrange
            var response = new ResponseContext { Status = HttpStatus.Ok};
            var request = new RequestContext { Path = "/resource" };

            // act
            _server.RegisterRoute("GET", "/resource", _ => response);
            _server.RegisterRoute("POST", "/resource", _ => response);

            request.Method = "GET";
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);

            request.Method = "POST";
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.ServeClient(_client.Object);

            // assert
            _client.Verify(client => client.GetRequest(), Times.Exactly(2));
            _client.Verify(client => client.SendResponse(response), Times.Exactly(2));
        }
    }
}