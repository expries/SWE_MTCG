using System.Security.Cryptography;
using MTCG.Domain;
using MTCG.Domain.Cards;
using MTCG.Domain.Cards.MonsterCards;
using MTCG.Domain.Cards.SpellCards;
using NUnit.Framework;

namespace MTCG.Test
{
    public class BattleLogicTest
    {
        private const string PlayerA = "PlayerA";
        private const string PlayerB = "PlayerB";
        
        [Test]
        public void Test_Fight_KnightLosesAgainstWaterSpell()
        {
            // arrange
            var knight = Knight.Create(20).Value;
            var waterSpell = WaterSpell.Create(1).Value;
            // act
            var log = BattleLogic.Fight(knight, waterSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(waterSpell, log.WinningCard);
        }

        [Test]
        public void Test_Fight_KnightWinsAgainstWeakerSpell()
        {
            // arrange
            var knight = Knight.Create(20).Value;
            var fireSpell = FireSpell.Create(1).Value;
            // act
            var log = BattleLogic.Fight(knight, fireSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(knight, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_KrakenIsImmuneToSpells()
        {
            // arrange
            var kraken = Kraken.Create(30).Value;
            var fireSpell = FireSpell.Create(100).Value;
            var waterSpell = WaterSpell.Create(100).Value;
            var normalSpell = RegularSpell.Create(100).Value;
            // act
            var logA = BattleLogic.Fight(kraken, fireSpell, PlayerA, PlayerB);
            var logB = BattleLogic.Fight(kraken, waterSpell, PlayerA, PlayerB);
            var logC = BattleLogic.Fight(kraken, normalSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(kraken, logA.WinningCard);
            Assert.AreEqual(kraken, logB.WinningCard);
            Assert.AreEqual(kraken, logC.WinningCard);
        }
        
        [Test]
        public void Test_Fight_NormalMonsterIsVulnerableToFireSpell()
        {
            // arrange
            var goblin = Goblin.Create(15).Value;
            var fireSpell = FireSpell.Create(15).Value;
            // act
            var log = BattleLogic.Fight(goblin, fireSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(fireSpell, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_NormalMonsterWinsAgainstWaterSpell()
        {
            // arrange
            var goblin = Goblin.Create(15).Value;
            var waterSpell = WaterSpell.Create(15).Value;
            // act
            var log = BattleLogic.Fight(goblin, waterSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(goblin, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_NormalMonsterWinsAgainstNormalSpell()
        {
            // arrange
            var goblin = Goblin.Create(15).Value;
            var normalSpell = RegularSpell.Create(10).Value;
            // act
            var log = BattleLogic.Fight(goblin, normalSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(goblin, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_FireMonsterIsVulnerableToWaterSpell()
        {
            // arrange
            var fireElf = FireElf.Create(25).Value;
            var waterSpell = WaterSpell.Create(20).Value;
            // act
            var log = BattleLogic.Fight(fireElf, waterSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(waterSpell, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_FireMonsterWinsAgainstNormalSpell()
        {
            // arrange
            var fireElf = FireElf.Create(25).Value;
            var normalSpell = RegularSpell.Create(20).Value;
            // act
            var log = BattleLogic.Fight(fireElf, normalSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(fireElf, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_FireMonsterWinsAgainstFireSpell()
        {
            // arrange
            var fireElf = FireElf.Create(25).Value;
            var fireSpell = FireSpell.Create(20).Value;
            // act
            var log = BattleLogic.Fight(fireElf, fireSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(fireElf, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_KrakenLosesAgainstStrongerMonster()
        {
            // arrange
            var kraken = Kraken.Create(10).Value;
            var dragon = Dragon.Create(20).Value;
            // act
            var log = BattleLogic.Fight(kraken, dragon, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(dragon, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_GoblinIsAfraidOfDragon()
        {
            // arrange
            var goblin = Goblin.Create(100).Value;
            var dragon = Dragon.Create(20).Value;
            // act
            var log = BattleLogic.Fight(goblin, dragon, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(dragon, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_GoblinWinsAgainstWeakerMonster()
        {
            // arrange
            var goblin = Goblin.Create(100).Value;
            var wizard = Wizard.Create(20).Value;
            // act
            var log = BattleLogic.Fight(goblin, wizard, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(goblin, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_FireElfDodgesDragon()
        {
            // arrange
            var fireElf = FireElf.Create( 10).Value;
            var dragon = Dragon.Create(50).Value;
            // act
            var log = BattleLogic.Fight(fireElf, dragon, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(fireElf, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_FireElfLosesAgainstStrongerMonster()
        {
            // arrange
            var fireElf = FireElf.Create(10).Value;
            var wizard = Wizard.Create(100).Value;
            // act
            var log = BattleLogic.Fight(fireElf, wizard, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(wizard, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_WizardControlsOrk()
        {
            // arrange
            var wizard = Wizard.Create(10).Value;
            var ork = Ork.Create(110).Value;
            // act
            var log = BattleLogic.Fight(wizard, ork, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(wizard, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_OrkWinsAgainstWeakerMonster()
        {
            // arrange
            var ork = Ork.Create(100).Value;
            var weakerMonster = FireElf.Create(20).Value;
            // act
            var log = BattleLogic.Fight(ork, weakerMonster, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(ork, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_WizardLosesAgainstStrongerMonster()
        {
            // arrange
            var wizard = Wizard.Create(20).Value;
            var strongerMonster = Dragon.Create(80).Value;
            // act
            var log = BattleLogic.Fight(wizard, strongerMonster, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(strongerMonster, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_MonsterLosesAgainstStrongerMonster()
        {
            // arrange
            var monster = Goblin.Create(20).Value;
            var strongerMonster = Goblin.Create(100).Value;
            // act
            var log = BattleLogic.Fight(monster, strongerMonster, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(strongerMonster, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_MonsterWinsAgainstWeakerMonster()
        {
            // arrange
            var monster = Goblin.Create(100).Value;
            var weakerMonster = Wizard.Create(20).Value;
            // act
            var log = BattleLogic.Fight(monster, weakerMonster, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(monster, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_AttackingMonsterLosesAgainstEquallyStrongMonster()
        {
            // arrange
            var wizard = Wizard.Create(20).Value;
            var monster = Goblin.Create(20).Value;
            // act
            var log = BattleLogic.Fight(wizard, monster, PlayerA, PlayerB);
            // assert
            Assert.IsNull(log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_MonsterLosesAgainstStrongerSpell()
        {
            // arrange
            var monster = Goblin.Create(50).Value;
            var spell = RegularSpell.Create(105).Value;
            // act
            var log = BattleLogic.Fight(monster, spell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(spell, log.WinningCard);
        }
        
        [Test]
        public void TestMonsterConstructor_NameIsGoblin()
        {
            // arrange
            // act
            var monster = Goblin.Create(0).Value;
            // assert
            Assert.AreEqual("Goblin", monster.Name);
        }
        
        [Test]
        public void TestMonsterConstructor_DamageIs10()
        {
            // arrange
            const int damage = 10;
            // act
            var monster = Goblin.Create(damage).Value;
            // assert
            Assert.AreEqual(damage, monster.Damage);
        }
        
        [Test]
        public void Test_Fight_MonsterWinsAgainstWeakerSpells()
        {
            // arrange
            var monster = Goblin.Create(50).Value;
            var spell = RegularSpell.Create(10).Value;
            // act
            var log = BattleLogic.Fight(monster, spell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(monster, log.WinningCard);
        }

        [Test]
        public void Test_Fight_FireSpellIsVulnerableToWaterSpell()
        {
            // arrange
            var fireSpell = FireSpell.Create(10).Value;
            var waterSpell = WaterSpell.Create(9).Value;
            // act
            var log = BattleLogic.Fight(fireSpell, waterSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(waterSpell, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_FireSpellWinsAgainstOtherSpell()
        {
            // arrange
            var fireSpell = FireSpell.Create(10).Value;
            var otherSpell = FireSpell.Create(9).Value;
            // act
            var log = BattleLogic.Fight(fireSpell, otherSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(fireSpell, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_WaterSpellIsVulnerableToNormalSpell()
        {
            // arrange
            var waterSpell = WaterSpell.Create(10).Value;
            var normalSpell = RegularSpell.Create(9).Value;
            // act
            var log = BattleLogic.Fight(waterSpell, normalSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(normalSpell, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_WaterSpellWinsAgainstOtherSpell()
        {
            // arrange
            var waterSpell = WaterSpell.Create(10).Value;
            var otherSpell = WaterSpell.Create(9).Value;
            // act
            var log = BattleLogic.Fight(waterSpell, otherSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(waterSpell, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_NormalSpellIsVulnerableToFireSpell()
        {
            // arrange
            var normalSpell = RegularSpell.Create(10).Value;
            var fireSpell = FireSpell.Create(9).Value;
            // act
            var log = BattleLogic.Fight(normalSpell, fireSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(fireSpell, log.WinningCard);
        }
        
        [Test]
        public void Test_Fight_NormalSpellWinsAgainstOtherSpell()
        {
            // arrange
            var regularSpell = RegularSpell.Create(15).Value;
            var otherSpell = RegularSpell.Create(10).Value;
            // act
            var log = BattleLogic.Fight(regularSpell, otherSpell, PlayerA, PlayerB);
            // assert
            Assert.AreEqual(regularSpell, log.WinningCard);
        }
    }
}