using MTCG.Repository;
using NUnit.Framework;

namespace MTCG.Test
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private UserRepository _repository;
        
        [SetUp]
        public void CreateRepository()
        {
            _repository = new UserRepository();
        }

        [Test]
        public void Test_CreateUser_ReturnsUser()
        {
            // arrange
            const string username = "kienboec";
            const string password = "daniel";
         
            // act
            var user = _repository.CreateUser(username, password);

            // assert
            Assert.AreEqual(username, user.Username);
            Assert.AreEqual(password, user.Password);
        }
        
        [Test]
        public void Test_CreateUser_ReturnsNullIfUsernameIsTaken()
        {
            // arrange
            const string username = "kienboec";
            const string password = "daniel";
         
            // act
            _repository.CreateUser(username, password);
            var user = _repository.CreateUser(username, password);

            // assert
            Assert.IsNull(user);
        }

        [Test]
        public void Test_GetUser_ReturnsUser()
        {
            // arrange
            const string username = "kienboec";
            const string password = "daniel";
         
            // act
            var newUser = _repository.CreateUser(username, password);
            var user = _repository.GetUser(username);

            // assert
            Assert.AreSame(newUser, user);
        }
        
        [Test]
        public void Test_GetUser_ReturnsNullIfNoUserIsFound()
        {
            // arrange
            const string username = "kienboec";
            
            // act
            var foundUser = _repository.GetUser(username);

            // assert
            Assert.IsNull(foundUser);
        }
        
        [Test]
        public void Test_CheckCredentials_ReturnsTrue()
        {
            // arrange
            const string username = "kienboec";
            const string password = "daniel";
            
            // act
            _repository.CreateUser(username, password);
            bool credCorrect = _repository.CheckCredentials(username, password);

            // assert
            Assert.IsTrue(credCorrect);
        }
        
        [Test]
        public void Test_CheckCredentials_InvalidCredentialsReturnFalse()
        {
            // arrange
            const string username = "kienboec";
            const string password = "daniel";
            
            // act
            bool credCorrect = _repository.CheckCredentials(username, password);

            // assert
            Assert.IsFalse(credCorrect);
        }
    }
}