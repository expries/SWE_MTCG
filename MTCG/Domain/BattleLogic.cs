using System;
using System.Collections.Generic;
using MTCG.Domain.Cards;

namespace MTCG.Domain
{
    public class BattleLogic
    {
        private static readonly List<Tuple<Element, Element>> ElementalEffectiveness = new()
        {
            new Tuple<Element, Element>(Element.Water, Element.Fire),
            new Tuple<Element, Element>(Element.Fire, Element.Normal),
            new Tuple<Element, Element>(Element.Normal, Element.Water),
        };
        
        private static readonly List<Tuple<MonsterType, MonsterType>> MonsterImmunity = new()
        {
            new Tuple<MonsterType, MonsterType>(MonsterType.FireElf, MonsterType.Dragon),
            new Tuple<MonsterType, MonsterType>(MonsterType.Wizard, MonsterType.Ork),
            new Tuple<MonsterType, MonsterType>(MonsterType.Dragon, MonsterType.Goblin),
        };

        private readonly BattleData _battleData;
        private readonly string _playerA;
        private readonly string _playerB;

        private BattleLogic(string playerA, string playerB)
        {
            _playerA = playerA;
            _playerB = playerB;
            _battleData = new BattleData();
        }

        public static BattleData Fight(Card cardA, Card cardB, string playerA, string playerB)
        {
            var instance = new BattleLogic(playerA, playerB);

            if (cardA.Type == cardB.Type && cardA.Type == CardType.Monster)
            {
                return instance.FightMonsterCards((MonsterCard) cardA, (MonsterCard) cardB);
            }

            return instance.FightCards(cardA, cardB);
        }

        private BattleData FightMonsterCards(MonsterCard cardA, MonsterCard cardB)
        {
            _battleData
                .Log(_playerA + ": " + cardA.Name + " (" + cardA.Damage + ")")
                .Log(" vs ")
                .Log(_playerB + ": " + cardB.Name + " (" + cardB.Damage + ")");
            
            if (IsImmune(cardA, cardB))
            {
                return _battleData
                    .AddWinner(cardA)
                    .Log(" => " + cardA.Name + " defeats " + cardB.Name + " (immune)")
                    .Log("\n");
            }
        
            if (IsImmune(cardB, cardA))
            {
                return _battleData
                    .AddWinner(cardB)
                    .Log(" => " + cardB.Name + " defeats " + cardA.Name + " (immune)")
                    .Log("\n");
            }

            if (cardA.Damage > cardB.Damage)
            {
                return _battleData
                    .AddWinner(cardA)
                    .Log(" => " + cardA.Name + " defeats " + cardB.Name)
                    .Log("\n");
            }

            if (cardB.Damage > cardA.Damage)
            {
                return _battleData
                    .AddWinner(cardB)
                    .Log(" => " + cardB.Name + " defeats " + cardA.Name)
                    .Log("\n");
            }
            
            return _battleData.Log(" => Draw").Log("\n");
        }
        
        private BattleData FightCards(Card cardA, Card cardB)
        {
            _battleData
                .Log(_playerA + ": " + cardA.Name + " (" + cardA.Damage + ")")
                .Log(" vs ")
                .Log(_playerB + ": " + cardB.Name + " (" + cardB.Damage + ")")
                .Log(" => " + cardA.Damage + " vs " + cardB.Damage);
            
            double damageA = cardA.Damage;
            double damageB = cardB.Damage;

            if (cardA.Type.Equals(CardType.Spell))
            {
                damageA *= GetElementalFactor(cardA, cardB);
            }

            if (cardB.Type.Equals(CardType.Spell))
            {
                damageA *= GetElementalFactor(cardB, cardA);
            }

            _battleData.Log(" -> " + cardA.Damage + " vs " + cardB.Damage);

            if (damageA > damageB)
            {
                return _battleData
                    .AddWinner(cardA)
                    .Log(" => " + cardA.Name + " wins")
                    .Log("\n");
            }

            if (damageB > damageA)
            {
                return _battleData
                    .AddWinner(cardA)
                    .Log(" => " + cardA.Name + " wins")
                    .Log("\n");
            }

            return _battleData.Log(" => Draw").Log("\n");
        }

        private static double GetElementalFactor(Card cardA, Card cardB)
        {
            if (IsEffective(cardA, cardB))
            {
                return 2;
            }

            if (IsIneffective(cardA, cardB))
            {
                return 0.5;
            }

            return 1;
        }

        private static bool IsImmune(MonsterCard cardA, MonsterCard cardB)
        {
            var pair = new Tuple<MonsterType, MonsterType>(cardA.MonsterType, cardB.MonsterType);
            return MonsterImmunity.Contains(pair);
        }

        private static bool IsEffective(Card cardA, Card cardB)
        {
            var pair = new Tuple<Element, Element>(cardA.Element, cardB.Element);
            return ElementalEffectiveness.Contains(pair);
        }

        private static bool IsIneffective(Card cardA, Card cardB)
        {
            return IsEffective(cardB, cardA);
        }
    }
}