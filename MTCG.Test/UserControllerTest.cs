using System;
using System.ComponentModel.Design.Serialization;
using System.Net.Mime;
using Moq;
using MTCG.Contracts.Requests;
using MTCG.Contracts.Responses;
using MTCG.Controller;
using MTCG.Server;
using MTCG.Server.TcpWrapper;
using MTCG.Service;
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
            Assert.AreEqual(user.Token.ToString(), response.Content);
        }
        
        [Test]
        public void Test_Register_ReturnsNullIfUserAlreadyExists()
        {
            // arrange
            var registrationRequest = new RegistrationRequest
            {
                Username = "kienboec",
                Password = "daniel"
            };
            var user = new User(registrationRequest.Username, registrationRequest.Password) {Token = Guid.NewGuid()};
            
            UserRepository.Setup(repository => repository.GetUser(registrationRequest.Username))
                .Returns(user);
            
            // act
            
            // assert
            Assert.Throws<ConflictException>(() => Controller.Register(registrationRequest));
        }
        
        [Test]
        public void Test_GetUser_ReturnsUser()
        {
            // arrange
            const HttpStatus status = HttpStatus.Ok;
            const string contentType = MediaType.Json;
            var user = new User("kienboec", "password") {Token = Guid.NewGuid()};
            
            UserRepository.Setup(repository => repository.GetUser(user.Username))
                .Returns(user);
            
            // act
            var response = Controller.Get(user.Username);
            var returnedUser = JsonConvert.DeserializeObject<User>(response.Content);

            // assert
            Assert.AreEqual(status, response.Status);
            Assert.AreEqual(contentType, response.ContentType);
            Assert.AreEqual(user.Username, returnedUser.Username);
            Assert.AreEqual(user.Password, returnedUser.Password);
            Assert.AreEqual(user.Token, returnedUser.Token);
        }
        
        [Test]
        public void Test_GetUser_IfUserNotFoundThrowsNotFoundException()
        {
            // arrange
            UserRepository.Setup(repository => repository.GetUser(It.IsAny<string>()))
                .Returns((User) null);
            
            // act
            // assert
            Assert.Throws<NotFoundException>(() => Controller.Get("kienboec"));
        }
    }
}