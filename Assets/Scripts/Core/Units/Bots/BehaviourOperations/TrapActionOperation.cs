using MageBattle.Core.Data;
using MageBattle.Core.Level;
using MageBattle.Core.Units.Spells;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourOperations
{
    public class TrapActionOperation : IBehaviourOperation
    {
        private const int _spellId = 5;
        private SpellInfo _spellInfo;

        public TrapActionOperation()
        {
            _spellInfo = SpellsInfoLoader.spellsInfo[_spellId];
        }

        public void Execute(Unit unit)
        {
            var unitsNearby = new List<Unit>();
            foreach (var unitToCheck in UnitsManager.instance.aliveUnits)
            {
                if (unit.data.userId == unitToCheck.data.userId)
                    continue;
                if (unit.isInvisible)
                    continue;
                if (LevelBuilder.instance.pathHelper.GetMaxAxisDistanceBetweenTiles(unit.currentTile, unitToCheck.currentTile) > _spellInfo.radius)
                    continue;
                unitsNearby.Add(unitToCheck);
            }
            if(unitsNearby.Count > 0)
            {
                Tile tileToCast;
                if(unitsNearby.Count == 1)
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