using System;
using MTCG.Repositories;
using MTCG.Resources.Cards.SpellCards;
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
            var requestedCard = new WaterSpell("WaterSpell", 10, 10) {Id = Guid.NewGuid()};

            // act
            var card = _repository.CreateCard(requestedCard);
            
            // assert
            Assert.AreEqual(requestedCard.Id, card.Id);
            Assert.AreEqual(requestedCard.Name, card.Name);
            Assert.AreEqual(requestedCard.Damage, card.Damage);
        }

        [Test]
        public void Test_CreateCard_ReturnsNullIfCardWithIdAlreadyExists()
        {
            // arrange
            var requestedCard = new WaterSpell("WaterSpell", 10, 10) {Id = Guid.NewGuid()};

            // act
            _repository.CreateCard(requestedCard);
            
            // assert
            Assert.Throws<ArgumentException>(() => _repository.CreateCard(requestedCard));
        }

        [Test]
        public void Test_GetCard_ReturnsCardIfItWasFound()
        {
            // arrange
            var requestedCard = new WaterSpell("WaterSpell", 10, 10) {Id = Guid.NewGuid()};

            // act
            _repository.CreateCard(requestedCard);
            var card = _repository.GetCard(requestedCard.Id);
            
            // assert
            Assert.AreEqual(requestedCard, card);
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
            var requestedCard = new WaterSpell("WaterSpell", 10, 10) {Id = Guid.NewGuid()};

            // act
            _repository.CreateCard(requestedCard);
            bool deleted = _repository.DeleteCard(requestedCard.Id);
            
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