using System;
using System.Collections.Generic;
using MTCG.Domain.Cards;

namespace MTCG.Domain
{
    public class BattleLogic
    {
        private readonly GameLog _log;
        private string _playerA;
        private string _playerB;
        
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

        private static readonly List<Tuple<MonsterType, Element>> ElementVulnerability = new()
        {
            new Tuple<MonsterType, Element>(MonsterType.Knight, Element.Water)
        };

        private static readonly List<MonsterType> SpellImmunity = new()
        {
            MonsterType.Kraken
        };
        
        private BattleLogic(string playerA, string playerB)
        {
            _playerA = playerA;
            _playerB = playerB;
            _log = new GameLog();
        }

        public static GameLog Fight(Card cardA, Card cardB, string playerA, string playerB)
        {
            var logic = new BattleLogic(playerA, playerB);
            return logic.CardFight(cardA, cardB);
        }

        private GameLog CardFight(Card cardA, Card cardB)
        {
            return cardA switch
            {
                MonsterCard monsterCardA => CardFight(monsterCardA, cardB),
                SpellCard spellCardA     => CardFight(spellCardA, cardB),
                _ => new GameLog().Append("Unknown card type: " + cardA.Type)
            };
        }
        
        private GameLog CardFight(SpellCard spellCardA, Card cardB)
        {
            return cardB switch
            {
                SpellCard spellCardB     => SpellFight(spellCardA, spellCardB),
                MonsterCard monsterCardB => MixedFight(spellCardA, monsterCardB),
                _ => new GameLog().Append("Unknown card type: " + cardB.Type)
            };
        }
        
        private GameLog CardFight(MonsterCard monsterCardA, Card cardB)
        {
            return cardB switch
            {
                MonsterCard monsterCardB => MonsterFight(monsterCardA, monsterCardB),
                SpellCard spellCardB     => MixedFight(monsterCardA, spellCardB),
                _ => new GameLog().Append("Unknown card type: " + cardB.Type)
            };
        }
        
        private GameLog MonsterFight(MonsterCard cardA, MonsterCard cardB)
        {
            _log.Append(_playerA + ": " + cardA.Name + " (" + cardA.Damage + ")")
                .Append(" vs ")
                .Append(_playerB + ": " + cardB.Name + " (" + cardB.Damage + ")");
            
            // check if a monster is immune against the other one
            if (IsImmune(cardA, cardB) && !IsImmune(cardB, cardA))
            {
                return _log.AddWinner(cardA)
                    .Append(" => " + cardA.Name + " defeats " + cardB.Name + " (immune)")
                    .Append("\n");
            }
        
            if (IsImmune(cardB, cardA) && !IsImmune(cardA, cardB))
            {
                return _log.AddWinner(cardB)
                    .Append(" => " + cardB.Name + " defeats " + cardA.Name + " (immune)")
                    .Append("\n");
            }

            // if no immunities: compare damage
            if (cardA.Damage > cardB.Damage)
            {
                return _log.AddWinner(cardA)
                    .Append(" => " + cardA.Name + " defeats " + cardB.Name)
                    .Append("\n");
            }
            
            if (cardB.Damage > cardA.Damage)
            {
                return _log.AddWinner(cardB)
                    .Append(" => " + cardB.Name + " defeats " + cardA.Name)
                    .Append("\n");
            }
            
            return _log.Append(" => Draw").Append("\n");
        }
        
        private GameLog MixedFight(MonsterCard cardA, SpellCard cardB)
        {
            (_playerA, _playerB) = (_playerB, _playerA);
            var battleLog = MixedFight(cardB, cardA);
            (_playerA, _playerB) = (_playerB, _playerA);
            return battleLog;
        }

        private GameLog MixedFight(SpellCard cardA, MonsterCard cardB)
        {
            _log.Append(_playerA + ": " + cardA.Name + " (" + cardA.Damage + ")")
                .Append(" vs ")
                .Append(_playerB + ": " + cardB.Name + " (" + cardB.Damage + ")");
            
            // monsters may be immune to spells
            if (IsSpellImmune(cardB))
            {
                return _log.AddWinner(cardB)
                    .Append(" => " + cardB.Name + " defeats " + cardA.Name + " (spell immune)")
                    .Append("\n");
            }

            // monsters may be vulnerable to a spell element
            if (IsVulnerable(cardB, cardA))
            {
                return _log.AddWinner(cardA)
                    .Append(" => " + cardA.Name + " defeats " + cardB.Name + " (vulnerable)")
                    .Append("\n");
            }
            
            return ElementalFight(cardA, cardB);
        }

        private GameLog SpellFight(Card cardA, Card cardB)
        {
            _log.Append(_playerA + ": " + cardA.Name + " (" + cardA.Damage + ")")
                .Append(" vs ")
                .Append(_playerB + ": " + cardB.Name + " (" + cardB.Damage + ")");

            return ElementalFight(cardA, cardB);
        }
        
        private GameLog ElementalFight(Card cardA, Card cardB)
        {
            _log.Append(" => " + cardA.Damage + " vs " + cardB.Damage);
            
            // use elemental effectiveness to card damage
            double damageA = cardA.Damage * GetElementalBonusFactor(cardA, cardB);
            double damageB = cardB.Damage * GetElementalBonusFactor(cardB, cardA);
            
            _log.Append(" -> " + damageA + " vs " + damageB);
            
            // compare (possibly elementally boosted) damage
            if (damageA > damageB)
            {
                return _log.AddWinner(cardA)
                    .Append(" => " + cardA.Name + " wins")
                    .Append("\n");
            }
            
            if (damageB > damageA)
            {
                return _log.AddWinner(cardB)
                    .Append(" => " + cardB.Name + " wins")
                    .Append("\n");
            }
            
            return _log.Append(" => Draw").Append("\n");
        }

        private static double GetElementalBonusFactor(Card cardA, Card cardB)
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
        
        private static bool IsIneffective(Card cardA, Card cardB)
        {
            return IsEffective(cardB, cardA);
        }

        private static bool IsEffective(Card cardA, Card cardB)
        {
            var pair = new Tuple<Element, Element>(cardA.Element, cardB.Element);
            return ElementalEffectiveness.Contains(pair);
        }
        
        private static bool IsSpellImmune(MonsterCard card)
        {
            return SpellImmunity.Contains(card.MonsterType);
        }

        private static bool IsVulnerable(MonsterCard cardA, SpellCard cardB)
        {
            var tuple = new Tuple<MonsterType, Element>(cardA.MonsterType, cardB.Element);
            return ElementVulnerability.Contains(tuple);
        }

        private static bool IsImmune(MonsterCard cardA, MonsterCard cardB)
        {
            var tuple = new Tuple<MonsterType, MonsterType>(cardA.MonsterType, cardB.MonsterType);
            return MonsterImmunity.Contains(tuple);
        }
    }
}