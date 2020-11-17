using System;
using System.Collections.Generic;
using Moq;
using MTCG.Controllers;
using MTCG.Exceptions;
using MTCG.Repositories;
using MTCG.Resources.Cards;
using MTCG.Resources.Cards.MonsterCards;
using MTCG.Resources.Cards.SpellCards;
using MTCG.Server;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MTCG.Test
{
    [TestFixture]
    public class CardControllerTest
    {
        private Mock<ICardRepository> CardRepository { get; set; }
        private CardController Controller { get; set; }

        [SetUp]
        public void CreateUserController()
        {
            CardRepository = new Mock<ICardRepository>();
            Controller = new CardController(CardRepository.Object);
        }

        [Test]
        public void Test_Get_ReturnsCard()
        {
            // arrange
            const HttpStatus status = HttpStatus.Ok;
            const string contentType = MediaType.Json;
            Card myCard = new NormalSpell("Spell", 200, 80) {Id = Guid.NewGuid()};

            CardRepository.Setup(repository => repository.GetCard(It.IsAny<Guid>()))
                .Returns(myCard);
            
            // act
            var response = Controller.Get(myCard.Id);
            Card returnedCard = JsonConvert.DeserializeObject<NormalSpell>(response.Content);

            // assert
            Assert.AreEqual(status, response.Status);
            Assert.AreEqual(contentType, response.ContentType);
            Assert.AreEqual(myCard.Damage, returnedCard.Damage);
            Assert.AreEqual(myCard.Element, returnedCard.Element);
            Assert.AreEqual(myCard.Id, returnedCard.Id);
            Assert.AreEqual(myCard.Name, returnedCard.Name);
        }
        
        [Test]
        public void Test_Get_ThrowsNotFoundExceptionIfCardDoesNotExist()
        {
            // arrange
            CardRepository.Setup(repository => repository.GetCard(It.IsAny<Guid>()))
                .Returns((Card) null);
            
            // act
            // assert
            Assert.Throws<NotFoundException>(() => Controller.Get(Guid.NewGuid()));
        }
        
        [Test]
        public void Test_GetAll_ReturnsEmptyCollection()
        {
            // arrange
            const HttpStatus status = HttpStatus.Ok;
            const string contentType = MediaType.Json;
            const string content = "[]";
            
            CardRepository.Setup(repository => repository.GetAllCards())
                .Returns(new List<Card>());
            
            // act
            var response = Controller.GetAll();
            
            // assert
            Assert.AreEqual(status, response.Status);
            Assert.AreEqual(contentType, response.ContentType);
            Assert.AreEqual(content, response.Content);
        }
        
        [Test]
        public void Test_GetAll_ReturnsMultipleCards()
        {
            // arrange
            const HttpStatus status = HttpStatus.Ok;
            const string contentType = MediaType.Json;

            var cardList = new List<Card>
            {
                new Dragon("dragon", 100), 
                new Dragon("otherDragon", 20)
            };

            CardRepository.Setup(repository => repository.GetAllCards())
                .Returns(cardList);
            
            // act
            var response = Controller.GetAll();
            var returnedCardList = JsonConvert.DeserializeObject<List<Dragon>>(response.Content);
            
            // assert
            Assert.AreEqual(status, response.Status);
            Assert.AreEqual(contentType, response.ContentType);
            Assert.AreEqual(cardList[0].Id, returnedCardList[0].Id);
            Assert.AreEqual(cardList[1].Id, returnedCardList[1].Id);
        }
    }
}