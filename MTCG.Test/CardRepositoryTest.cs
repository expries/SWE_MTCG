using System;
using System.Collections.Generic;
using MTCG.Contracts.Requests;
using MTCG.Repository;
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
                Name = "Goblin",
                Damage = 10.0,
                Weakness = 10.0,
                Id = Guid.NewGuid()
            };
            
            // act
            var card = _repository.Create(Id, Name, Damage, Weakness);
            
            // assert
            Assert.AreEqual(request.Id, card.Id);
            Assert.AreEqual(request.Name, card.Name);
            Assert.AreEqual(request.Damage, card.Damage);
            Assert.AreEqual(request.Weakness, card.Weakness);
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
        public void Test_CreateCardCollection_ReturnsNumberOfCreatedCards()
        {
            // arrange
            var requestList = new List<CardCreationRequest>();
            for (int i = 0; i < 10; i++)
            {
                requestList.Add(new CardCreationRequest
                {
                    Id = Guid.NewGuid(),
                    Damage = 10.0 * i,
                    Name = "Goblin " + i.ToString(),
                    Weakness = 30.0
                });
            }
            
            // act
            int createdCards = _repository.CreateCardCollection(requestList);
            
            // assert
            Assert.AreEqual(10, createdCards);
        }
        
        [Test]
        public void Test_GetCard_ReturnsCardIfItWasFound()
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
            var card = _repository.GetCard(request.Id);
            
            // assert
            Assert.AreEqual(request.Id, card.Id);
            Assert.AreEqual(request.Name, card.Name);
            Assert.AreEqual(request.Damage, card.Damage);
            Assert.AreEqual(request.Weakness, card.Weakness);
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
        public bool Test_DeleteCard_ReturnsTrueIfCardWasDeleted()
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
            bool deleted = _repository.DeleteCard(request.Id);
            
            // assert
            Assert.IsTrue(deleted);
        }
        
        [Test]
        public bool Test_DeleteCard_ReturnsFalseIfCardDoesNotExist()
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