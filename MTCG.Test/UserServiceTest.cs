using System;
using Moq;
using MTCG.Contracts.Requests;
using MTCG.Repositories;
using MTCG.Resources;
using MTCG.Services;
using NUnit.Framework;

namespace MTCG.Test
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IUserRepository> _repository;
        private UserService _service;

        [SetUp]
        public void CreateService()
        {
            _repository = new Mock<IUserRepository>();
            _service = new UserService(_repository.Object);
        }

        [Test]
        public void Test_Register_ReturnsUser()
        {
            // arrange
            var request = new RegistrationRequest { Username = "kienboec", Password = "daniel" };
            var user = new User(request.Username, request.Password);
            user.Token = Guid.NewGuid().ToString();

            _repository.Setup(repo => repo.CreateUser(user.Username, user.Password))
                .Returns(user);
            _repository.Setup(repo => repo.GetUser(It.IsAny<string>()))
                .Returns((User) null);
            
            // act
            var registeredUser = _service.Register(request);

            // assert
            Assert.IsTrue(registeredUser.Success);
            Assert.AreEqual(user.Token, registeredUser.Token);
        }

        [Test]
        public void Test_Register_UsernameTokenReturnsError()
        {
            // arrange
            var request = new RegistrationRequest { Username = "kienboec", Password = "daniel" };
            var user = new User(request.Username, request.Password);
            
            _repository.Setup(repo => repo.GetUser(It.IsAny<string>()))
                .Returns(user);

            // act
            var response = _service.Register(request);

            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }

        [Test]
        public void Test_Register_UsernameIsEmptyReturnsError()
        {
            // arrange
            var request = new RegistrationRequest { Username = "", Password = "daniel" };

            // act
            var response = _service.Register(request);
            
            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }
        
        [Test]
        public void Test_Register_UsernameIsNullReturnsError()
        {
            // arrange
            var request = new RegistrationRequest { Username = null, Password = "daniel" };

            // act
            var response = _service.Register(request);
            
            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }
        
        [Test]
        public void Test_Register_PasswordIsEmptyReturnsError()
        {
            // arrange
            var request = new RegistrationRequest { Username = "kienboec", Password = "" };

            // act
            var response = _service.Register(request);
            
            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }

        [Test]
        public void Test_Register_PasswordIsNullReturnsError()
        {
            // arrange
            var request = new RegistrationRequest { Username = "kienboec", Password = null };

            // act
            var response = _service.Register(request);
            
            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }

        [Test]
        public void Test_Login_ReturnsSuccess()
        {
            // arrange
            var request = new LoginRequest { Username = "kienboec", Password = "daniel" };

            _repository.Setup(repo =>
                    repo.CheckCredentials(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            
            // act
            var response = _service.Login(request);
            
            // assert
            Assert.IsTrue(response.Success);
        }

        [Test]
        public void Test_Login_ReturnsFalse()
        {
            // arrange
            var request = new LoginRequest { Username = "kienboec", Password = "daniel" };

            _repository.Setup(repo =>
                    repo.CheckCredentials(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            
            // act
            var response = _service.Login(request);

            // assert
            Assert.IsFalse(response.Success);
        }

        [Test]
        public void Test_Login_ReturnsErrorIfUsernameIsEmpty()
        {
            // arrange
            var request = new LoginRequest { Username = "", Password = "daniel" };

            // act
            var response = _service.Login(request);

            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }
        
        [Test]
        public void Test_Login_ReturnsErrorIfUsernameIsNull()
        {
            // arrange
            var request = new LoginRequest { Username = null, Password = "daniel" };

            // act
            var response = _service.Login(request);

            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }
        
        [Test]
        public void Test_Login_ReturnsErrorIfPasswordIsEmpty()
        {
            // arrange
            var request = new LoginRequest { Username = "kienboec", Password = "" };

            // act
            var response = _service.Login(request);

            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }
        
        [Test]
        public void Test_Login_ReturnsErrorIfPasswordIsNull()
        {
            // arrange
            var request = new LoginRequest { Username = "kienboec", Password = null };

            // act
            var response = _service.Login(request);

            // assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Error);
        }

        [Test]
        public void Test_SessionExists_ReturnsTrueIfSessionExists()
        {
            // arrange
            var user = new User("kienboec", "daniel");
            user.Token = Guid.NewGuid().ToString();
            
            _repository.Setup(repo => repo.GetUser(It.IsAny<string>()))
                .Returns(user);
            
            // act
            bool tokenIsOk = _service.CheckSession(user.Username, user.Token);
            
            // assert
            Assert.IsTrue(tokenIsOk);
        }
        
        [Test]
        public void Test_SessionExists_ReturnsFalseIfSessionDoesNotExist()
        {
            // arrange
            var user = new User("kienboec", "daniel");
            user.Token = Guid.NewGuid().ToString();
            
            _repository.Setup(repo => repo.GetUser(It.IsAny<string>()))
                .Returns(() =>
                {
                    var wrongUser = user;
                    wrongUser.Token = Guid.NewGuid().ToString();
                    return wrongUser;
                });
            
            // act
            bool tokenIsOk = _service.CheckSession(user.Username, user.Token);
            
            // assert
            Assert.IsFalse(tokenIsOk);
        }
    }
}