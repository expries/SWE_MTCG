using System;
using System.ComponentModel.Design.Serialization;
using System.Net.Mime;
using Moq;
using MTCG.Contracts.Requests;
using MTCG.Contracts.Responses;
using MTCG.Controllers;
using MTCG.Exceptions;
using MTCG.Resources;
using MTCG.Server;
using MTCG.Server.TcpWrapper;
using MTCG.Services;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace MTCG.Test
{
    [TestFixture]
    public class UserControllerTest
    {
        private Mock<IUserService> Service { get; set; }
        private UserController Controller { get; set; }

        [SetUp]
        public void CreateUserController()
        {
            Service = new Mock<IUserService>();
            Controller = new UserController(Service.Object);
        }

        [Test]
        public void Test_Register_ReturnsTokenIfRegistrationWasSuccessful()
        {
            // arrange
            const HttpStatus status = HttpStatus.Created;
            const string contentType = MediaType.Plaintext;
            
            var registrationRequest = new RegistrationRequest
            {
                Username = "kienboec",
                Password = "daniel"
            };
            var registrationResponse = new RegistrationResponse
            {
                Success = true, 
                Token = Guid.NewGuid().ToString()
            };

            Service.Setup(service => service.Register(It.IsAny<RegistrationRequest>()))
                .Returns(registrationResponse);
            
            // act
            var response = Controller.Register(registrationRequest);

            // assert
            Assert.AreEqual(status, response.Status);
            Assert.AreEqual(contentType, response.ContentType);
            Assert.AreEqual(registrationResponse.Token, response.Content);
        }
    }
}