﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cookie.API.Core;
using Cookie.API.Game.Fight;
using Cookie.API.Game.Fight.Fighters;
using Cookie.API.Gamedata;
using Cookie.API.Protocol.Network.Messages.Game.Context.Fight;
using Cookie.API.Utils;
using Cookie.Core.Scripts;
using Cookie.Game.Fight.Spell;
using Cookie.API.Game.World.Pathfinding.Positions;
using System;

namespace Cookie.Game.Fight
{
    public class ArtificialIntelligence : IArtificialIntelligence
    {
        private IAccount _account;
        private List<IASpell> _spells;
        private int TotalSpellLauch;
        private SpellCast SpellEvent;

        public void Load(IAccount account, string path)
        {
            TotalSpellLauch = 0;
            _account = account;
            _spells = ScriptsManager.LoadSpellsFromIA($"{path}.lua");

            var spellsId = _account.Character.Spells.Select(s => s.SpellId).ToList();

            foreach (var spell in _spells)
            {
                if (spellsId.Contains(spell.SpellId)) continue;
                var spellTmp = _spells.FirstOrDefault(s => s.SpellId == spell.SpellId);
                if (spellTmp != null)
                    _spells.Remove(spellTmp);
            }

            foreach (var spell in _spells)
            {
                Logger.Default.Log(spell.SpellId.ToString());
            }

            _account.Character.Fight.FightStarted += StartFight;
            _account.Character.Fight.TurnStarted += ExecuteSpell;
            _account.Character.Fight.FightEnded += FightEnd;
        }

        private void FightEnd(GameFightEndMessage msg)
        {
            Logger.Default.Log($"Durée du combat: {TimeSpan.FromMilliseconds(msg.Duration).TotalSeconds} secondes");
        }

        private void StartFight()
        {
            _account.Character.Fight.SetReady();
        }

        private void ExecuteSpell()
        {
            Logger.Default.Log($"Vie du bot: {_account.Character.LifePercentage}");
            if (_spells == null) return;
            var monster = _account.Character.Fight.NearestMonster();
            //foreach (var spell in _spells)
            //{
            var spell = _spells[0];
            var fighter = (IFighter)monster;
            if (spell.Target == SpellTarget.Self)
                fighter = _account.Character.Fight.Fighter;
            var useSpell = _account.Character.Fight.CanUseSpell(spell.SpellId, fighter);
            var nameSpell = D2OParsing.GetSpellName(spell.SpellId);
            if (TotalSpellLauch <= spell.Relaunchs)
            {
                Logger.Default.Log($"Attaque {monster.Name}");
                switch (useSpell)
                {
                    case -1:
                        _account.Character.Fight.EndTurn();
                        break;

                    case 0:
                        Logger.Default.Log($"Lancement de {nameSpell}");
                        var spellEvent = new SpellCast(_account, spell.SpellId, fighter.CellId);
                        spellEvent.SpellCasted += (sender, e) =>
                        {
                            TotalSpellLauch++;
                            if (TotalSpellLauch < spell.Relaunchs)
                                ExecuteSpell();
                            else
                            {
                                TotalSpellLauch = 0;
                                _account.Character.Fight.EndTurn();
                            }
                        };
                        spellEvent.PerformCast();
                        break;

                    default:
                        Logger.Default.Log($"Déplacement en {useSpell}");
                        var movement = _account.Character.Fight.MoveToCell(useSpell);
                        movement.MovementFinished += (sender, e) =>
                        {
                            if (e.Sucess)
                            {
                                Logger.Default.Log($"Lancement de {nameSpell}");
                                var sspellEvent = new SpellCast(_account, spell.SpellId, fighter.CellId);
                                sspellEvent.SpellCasted += (sendere, ee) =>
                                {
                                    TotalSpellLauch++;
                                    if (TotalSpellLauch < spell.Relaunchs)
                                        ExecuteSpell();
                                    else
                                    {
                                        TotalSpellLauch = 0;
                                        _account.Character.Fight.EndTurn();
                                    }
                                };
                                sspellEvent.PerformCast();
                            }
                            else
                                Logger.Default.Log($"Erreur lors du lancement du spell {spell.SpellId} sur la cell {fighter.CellId}", API.Utils.Enums.LogMessageType.Public);
                        };
                        movement.PerformMovement();
                        break;
                }
            }
            else
            {
                TotalSpellLauch = 0;
                _account.Character.Fight.EndTurn();
            }
        }
    }
}