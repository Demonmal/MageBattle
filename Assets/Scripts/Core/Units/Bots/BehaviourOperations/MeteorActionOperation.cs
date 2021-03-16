using MageBattle.Core.Data;
using MageBattle.Core.Level;
using MageBattle.Core.Units.Spells;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourOperations
{
    public class MeteorActionOperation : IBehaviourOperation
    {
        private const int _spellId = 6;
        private SpellInfo _spellInfo;

        public MeteorActionOperation()
        {
            _spellInfo = SpellsInfoLoader.spellsInfo[_spellId];
        }

        public void Execute(Unit unit)
        {
            var unitsNearby = new List<Unit>();
            foreach (var unitToCheck in UnitsManager.instance.aliveUnits)
            {
                if (unitToCheck.data.userId == unit.data.userId)
                    continue;
                if (unit.isInvisible)
                    continue;
                if (LevelBuilder.instance.pathHelper.GetMaxAxisDistanceBetweenTiles(unit.currentTile, unitToCheck.currentTile) > _spellInfo.radius)
                    continue;
                if (unitToCheck.health <= DamageValuesContainer.damageBySource[Enums.DamageSource.Meteor])
                {
                    unit.actionsExecutor.Cast(_spellId, unitToCheck.currentTile);
                    return;
                }
                else
                {
                    unitsNearby.Add(unitToCheck);
                }
            }
            if (unitsNearby.Count > 0)
            {
                Tile tileToCast;
                if (unitsNearby.Count == 1)
                {
                    tileToCast = unitsNearby[0].currentTile;
                }
                else
                {
                    tileToCast = unitsNearby[Random.Range(0, unitsNearby.Count - 1)].currentTile;
                }
                unit.actionsExecutor.Cast(_spellId, tileToCast);
            }
        }
    }
}