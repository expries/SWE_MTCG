using System;
using System.Collections.Generic;
using MTCG.Contracts.Requests;
using MTCG.Repositories;
using NUnit.Framework;

namespace MTCG.Test
{
    [TestFixture]
    public class CardRepositoryTest
    {
        private CardRepository _repository;

        [SetUp]
        public void CreateRepository()
        {
            _repository = new CardRepository();
        }

        [Test]
        public void Test_CreateCard_ReturnsNewCardOnCreation()
        {
            // arrange
            var request = new CardCreationRequest
            {
                Name = "WaterSpell",
                Damage = 10.0,
                Weakness = 10.0,
                Id = Guid.NewGuid()
            };
            
            // act
            var card = _repository.CreateCard(request.Id, request.Name, request.Damage, request.Weakness);
            
            // assert
            Assert.AreEqual(request.Id, card.Id);
            Assert.AreEqual(request.Name, card.Name);
            Assert.AreEqual(request.Damage, card.Damage);
        }

        [Test]
        public void Test_CreateCard_ReturnsNullIfCardWithIdAlreadyExists()
        {
            // arrange
            var request = new CardCreationRequest
            {
                Name = "Goblin",
                Damage = 10.0,
                Weakness = 10.0,
                Id = Guid.NewGuid()
            };
            
            // act
            _repository.CreateCard(request.Id, request.Name, request.Damage, request.Weakness);
            var card = _repository.CreateCard(request.Id, request.Name, request.Damage, request.Weakness);
            
            // assert
            Assert.IsNull(card);
        }

        [Test]
        public void Test_GetCard_ReturnsCardIfItWasFound()
        {
            // arrange
            var request = new CardCreationRequest
            {
                Name = "Goblin",
                Damage = 10.0,
                Id = Guid.NewGuid()
            };

            // act
            _repository.CreateCard(request.Id, request.Name, request.Damage);
            var card = _repository.GetCard(request.Id);
            
            // assert
            Assert.AreEqual(request.Id, card.Id);
            Assert.AreEqual(request.Name, card.Name);
            Assert.AreEqual(request.Damage, card.Damage);
        }

        [Test]
        public void Test_GetCard_ReturnsNullIfCardWithIdDoesNotExist()
        {
            // arrange
            var id = Guid.NewGuid();

            // act
            var card = _repository.GetCard(id);
            
            // assert
            Assert.IsNull(card);
        }
        
        [Test]
        public void Test_DeleteCard_ReturnsTrueIfCardWasDeleted()
        {
            // arrange
            var request = new CardCreationRequest
            {
                Name = "WaterSpell",
                Damage = 10.0,
                Weakness = 10.0,
                Id = Guid.NewGuid()
            };

            // act
            _repository.CreateCard(request.Id, request.Name, request.Damage, request.Weakness);
            bool deleted = _repository.DeleteCard(request.Id);
            
            // assert
            Assert.IsTrue(deleted);
        }
        
        [Test]
        public void Test_DeleteCard_ReturnsFalseIfCardDoesNotExist()
        {
            // arrange
            var id = Guid.NewGuid();

            // act
            bool deleted = _repository.DeleteCard(id);
            
            // assert
            Assert.IsFalse(deleted);
        }
    }
}