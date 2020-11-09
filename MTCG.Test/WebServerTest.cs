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
        private Mock<ITcpListener> _tcpListener;
        private Mock<ITcpClient> _client;
        private WebServer _server;

        [SetUp]
        public void CreateMocksAndServer()
        {
            _tcpListener = new Mock<ITcpListener>();
            _client = new Mock<ITcpClient>();
            _server = new WebServer();
        }

        [SetUp]
        public void SetupMockMethods()
        {
            _client.Setup(client => client.GetRequest())
                .Returns(() => new RequestContext());

            _client.Setup(client => client.SendResponse(It.IsAny<ResponseContext>()))
                .Callback(() => {});

            _client.Setup(client => client.Close())
                .Callback(() => {});

            _tcpListener.Setup(tcpListener => tcpListener.Start())
                .Callback(() => {});

            _tcpListener.Setup(tcpListener => tcpListener.AcceptTcpClient())
                .Returns(() => _client.Object);
        }

        [Test]
        public void Test_Listen_TcpListener_StartsAndCallsAcceptClient()
        {
            // arrange

            // act
            _server.Listen(_tcpListener.Object);
            
            // assert
            _tcpListener.Verify(x => x.Start(), Times.Once);
            _tcpListener.Verify(x => x.AcceptTcpClient(), Times.Once);
        }
        
        [Test]
        public void Test_Listen_ParameterlessRouteInvoked()
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
            _server.Listen(_tcpListener.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(response), Times.Once);
        }
        
        [Test]
        public void Test_Listen_ParameterRouteInvoked()
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
            _server.Listen(_tcpListener.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Content.Equals(resourceId))));
        }
        
        [Test]
        public void Test_Listen_MultiParameterRouteInvoked()
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
            _server.Listen(_tcpListener.Object);    
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Content.Equals(responseContent))));
        }
        
        [Test]
        public void Test_Listen_EndpointNotFound()
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
            _server.Listen(_tcpListener.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_Listen_HttpVerbNotSupportedForRoute()
        {
            // arrange
            const HttpStatus status = HttpStatus.NotImplemented;
            var request = new RequestContext
            {
                Method = "TRACE",
                Path = "/resource"
            };

            // act
            _server.RegisterRoute("GET", "/resource", _ => null);
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.Listen(_tcpListener.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_Listen_ResourceNotFound()
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
            _server.Listen(_tcpListener.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_Listen_BadFormatException()
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
            _server.Listen(_tcpListener.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_Listen_InternalServerErrorException()
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
            _server.Listen(_tcpListener.Object);
            
            // assert
            _client.Verify(client => client.GetRequest(), Times.Once);
            _client.Verify(client => client.SendResponse(
                It.Is<ResponseContext>(response => response.Status.Equals(status))));
        }
        
        [Test]
        public void Test_Listen_MultipleMethodsOnSameEndpoint()
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
            _server.Listen(_tcpListener.Object);

            request.Method = "POST";
            _client.Setup(client => client.GetRequest())
                .Returns(() => request);
            _server.Listen(_tcpListener.Object);

            // assert
            _client.Verify(client => client.GetRequest(), Times.Exactly(2));
            _client.Verify(client => client.SendResponse(response), Times.Exactly(2));
        }
    }
}