using MTCG.Resources.Cards;
using MTCG.Resources.Cards.MonsterCards;
using MTCG.Resources.Cards.SpellCards;
using NUnit.Framework;

namespace MTCG.Test
{
    public class CardTest
    {
        [Test]
        public void Test_AttackedBy_KnightLosesAgainstWaterSpell()
        {
            var knight = new Knight("Alan", 20);
            var waterSpell = new WaterSpell("Water!", 1);
            bool fight = waterSpell.Attack(knight);
            Assert.IsTrue(fight);
        }

        [Test]
        public void Test_AttackedBy_KnightWinsAgainstWeakerSpell()
        {
            var card = new Knight("Alan", 20);
            var fireSpell = new FireSpell("Fire!", 1);
            bool fight = fireSpell.Attack(card);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_KrakenIsImmuneToSpells()
        {
            var kraken = new Kraken("George", 30);
            var fireSpell = new FireSpell("Fire!", 100);
            var waterSpell = new WaterSpell("Water!", 100);
            var normalSpell = new NormalSpell("Normal!", 100);
        
            bool fightNormalSpell = normalSpell.Attack(kraken);
            bool fightFireSpell = fireSpell.Attack(kraken);
            bool fightWaterSpell = waterSpell.Attack(kraken);
            
            Assert.IsFalse(fightNormalSpell);
            Assert.IsFalse(fightFireSpell);
            Assert.IsFalse(fightWaterSpell);
        }
        
        [Test]
        public void Test_AttackedBy_NormalMonsterIsVulnerableToFireSpell()
        {
            var goblin = new Goblin("Simon", 15);
            var fireSpell = new FireSpell("Fire!", 15);
            bool fight = fireSpell.Attack(goblin);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_NormalMonsterWinsAgainstWaterSpell()
        {
            var goblin = new Goblin("Simon", 15);
            var waterSpell = new WaterSpell("Water!", 15);
            bool fight = waterSpell.Attack(goblin);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_NormalMonsterWinsAgainstNormalSpell()
        {
            var goblin = new Goblin("Simon", 15);
            var normalSpell = new NormalSpell("Normal!", 15);
            bool fight = normalSpell.Attack(goblin);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_FireMonsterIsVulnerableToWaterSpell()
        {
            var fireElf = new FireElf("Angela", 25);
            var waterSpell = new WaterSpell("Water!", 20);
            bool fight = waterSpell.Attack(fireElf);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_FireMonsterWinsAgainstNormalSpell()
        {
            var fireElf = new FireElf("Angela", 25);
            var normalSpell = new NormalSpell("Normal!", 20);
            bool fight = fireElf.Attack(normalSpell);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_FireMonsterWinsAgainstFireSpell()
        {
            var fireElf = new FireElf("Angela", 25);
            var fireSpell = new FireSpell("Fire!", 20);
            bool fight = fireSpell.Attack(fireElf);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_KrakenLosesAgainstStrongerMonster()
        {
            var kraken = new Kraken("George", 10);
            var dragon = new Dragon("Andew", 20);
            bool fight = dragon.Attack(kraken);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_GoblinIsAfraidOfDragon()
        {
            var goblin = new Goblin("Simon", 100);
            var dragon = new Dragon("Andrew", 20);
            bool fight = goblin.Attack(dragon);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_GoblinWinsAgainstWeakerMonster()
        {
            Card goblin = new Goblin("Simon", 100);
            Card wizard = new Wizard("Merlin", 20);
            bool fight = goblin.Attack(wizard);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_FireElfDodgesDragon()
        {
            var fireElf = new FireElf("Angela", 10);
            var dragon = new Dragon("Andrew", 50);
            bool fight = dragon.Attack(fireElf);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_FireElfLosesAgainstStrongerMonster()
        {
            var fireElf = new FireElf("Angela", 10);
            var wizard = new Wizard("Merlin", 100);
            bool fight = wizard.Attack(fireElf);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_WizardControlsOrk()
        {
            var wizard = new Wizard("Merlin", 10);
            var ork = new Ork("...", 110);
            bool fight = ork.Attack(wizard);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_OrkWinsAgainstWeakerMonster()
        {
            var ork = new Ork("...", 100);
            var weakerMonster = new FireElf("Angela", 20);
            bool fight = ork.Attack(weakerMonster);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_WizardLosesAgainstStrongerMonster()
        {
            var wizard = new Wizard("Merlin", 20);
            var strongerMonster = new Dragon("Andew", 80);
            bool fight = strongerMonster.Attack(wizard);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_MonsterLosesAgainstStrongerMonster()
        {
            var monster = new Goblin("Simon", 20);
            var strongerMonster = new Goblin("Simon", 100);
            bool fight = strongerMonster.Attack(monster);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_MonsterWinsAgainstWeakerMonster()
        {
            var monster = new Goblin("Simon", 100);
            var weakerMonster = new Wizard("Merlin", 20);
            bool fight = weakerMonster.Attack(monster);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_AttackingMonsterLosesAgainstEquallyStrongMonster()
        {
            var wizard = new Wizard("Merlin", 20);
            var monster = new Goblin("Simon", 20);
            bool fight = wizard.Attack(monster);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_MonsterLosesAgainstStrongerSpell()
        {
            var monster = new Goblin("Simon", 50);
            Card spell = new NormalSpell("Normal!", 105);
            bool fight = spell.Attack(monster);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void TestMonsterConstructor_NameIsSimon()
        {
            const string name = "Simon";
            var monster = new Goblin(name, 0);
            Assert.AreEqual(name, monster.Name);
        }
        
        [Test]
        public void TestMonsterConstructor_DamageIs10()
        {
            const int damage = 10;
            var monster = new Goblin("Simon", damage);
            Assert.AreEqual(damage, monster.Damage);
        }
        
        [Test]
        public void Test_AttackedBy_MonsterWinsAgainstWeakerSpells()
        {
            Card monster = new Goblin("Simon", 50);
            Card spell = new NormalSpell("Normal!", 10);
            bool fight = spell.Attack(monster);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_AttackingSpellLosesAgainstEquallyStrongMonster()
        {
            Card monster = new Goblin("Simon", 20);
            Card spell = new WaterSpell("Water!", 20);
            bool fight = spell.Attack(monster);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_FireSpellIsVulnerableToWaterSpell()
        {
            Card fireSpell = new FireSpell("Fire!", 10);
            Card waterSpell = new WaterSpell("Water!", 9);
            bool fight = waterSpell.Attack(fireSpell);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_FireSpellWinsAgainstOtherSpell()
        {
            Card fireSpell = new FireSpell("Fire!", 10);
            Card otherSpell = new FireSpell("Fire!", 9);
            bool fight = otherSpell.Attack(fireSpell);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_WaterSpellIsVulnerableToNormalSpell()
        {
            Card waterSpell = new WaterSpell("Water!", 10);
            Card normalSpell = new NormalSpell("Normal!", 9);
            bool fight = normalSpell.Attack(waterSpell);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_WaterSpellWinsAgainstOtherSpell()
        {
            var waterSpell = new WaterSpell("Water!", 10);
            var otherSpell = new WaterSpell("Water!", 9);
            bool fight = otherSpell.Attack(waterSpell);
            Assert.IsFalse(fight);
        }
        
        [Test]
        public void Test_AttackedBy_NormalSpellIsVulnerableToFireSpell()
        {
            var normalSpell = new NormalSpell("Normal!", 10);
            var fireSpell = new FireSpell("Fire!", 9);
            bool fight = fireSpell.Attack(normalSpell);
            Assert.IsTrue(fight);
        }
        
        [Test]
        public void Test_AttackedBy_NormalSpellWinsAgainstOtherSpell()
        {
            var normalSpell = new NormalSpell("Normal!", 10);
            var otherSpell = new NormalSpell("Normal!", 10);
            bool fight = otherSpell.Attack(normalSpell);
            Assert.IsFalse(fight);
        }
    }
}