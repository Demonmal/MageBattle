using MageBattle.Core.Data;
using MageBattle.Core.Level;
using MageBattle.Core.Units.Bots.BehaviourOperations;
using MageBattle.Core.Units.Spells;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourPriorities
{
    public class MeteorActionBehaviour : IBotBehaviour
    {
        private Unit _currentUnit;
        private BotData _botData;
        private SpellInfo _spellInfo;

        private const float _defaultPriorityMultiplier = 0.2f;
        private const float _enemyWithLowHpMultiplier = 0.2f;
        private const float _patternMultiplier = 0.4f;

        private const int _spellId = 6;

        public IBehaviourOperation operation { get; private set; }
        public bool isAbsoluteChoice { get; private set; } = false;

        public MeteorActionBehaviour()
        {
            _spellInfo = SpellsInfoLoader.spellsInfo[_spellId];
            operation = new MeteorActionOperation();
        }

        public int GetPriority(Unit unit)
        {
            SetCurrentUnit(unit);
            if (IsAvailableForUnit(unit))
            {
                int fullPriority = BotsDecisionMaker.GetPriorityByMultiplyer(_defaultPriorityMultiplier) + GetEnemyWithLowHpPriority() + GetPatternPriority();
                return Mathf.Clamp(fullPriority, 0, BotsDecisionMaker.maxPriority);
            }
            else
            {
                return 0;
            }
        }

        public bool CanReachGem(Unit unit)
        {
            return false;
        }

        private int GetEnemyWithLowHpPriority()
        {
            int priority = 0;
            foreach (var unit in UnitsManager.instance.aliveUnits)
            {
                if (unit.data.userId == _currentUnit.data.userId)
                    continue;
                if (LevelBuilder.instance.pathHelper.GetMaxAxisDistanceBetweenTiles(_currentUnit.currentTile, unit.currentTile) > _spellInfo.radius)
                    continue;
                if (unit.health <= DamageValuesContainer.damageBySource[Enums.DamageSource.Meteor])
                {
                    priority = BotsDecisionMaker.GetPriorityByMultiplyer(_enemyWithLowHpMultiplier);
                    break;
                }
            }
            return priority;
        }

        private int GetPatternPriority()
        {
            var pattern = _botData.behaviourPattern;
            int priority = pattern == BotBehaviourPatern.Agressor ? BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier)
                : -BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier);
            return priority;
        }

        public bool IsAvailableForUnit(Unit unit)
        {
            SetCurrentUnit(unit);
            return unit.actionsExecutor.CanUseAction(Actions.ActionType.Cast, _spellId) && AnyUnitToCastExistsInRange();
        }

        private bool AnyUnitToCastExistsInRange()
        {
            (int, int) xRange = (_currentUnit.currentTile.x - _spellInfo.radius, _currentUnit.currentTile.x + _spellInfo.radius);
            (int, int) zRange = (_currentUnit.currentTile.z - _spellInfo.radius, _currentUnit.currentTile.z + _spellInfo.radius);
            var tiles = LevelBuilder.instance.GetAllTilesInRanges(xRange, zRange);
            foreach (var tile in tiles)
            {
                if (tile.id == _currentUnit.currentTile.id)
                    continue;
                if (LevelBuilder.instance.pathHelper.GetMaxAxisDistanceBetweenTiles(_currentUnit.currentTile, tile) > _spellInfo.radius)
                    continue;
                if (tile.type == TileType.With_Player)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetCurrentUnit(Unit unit)
        {
            if (_currentUnit == unit)
                return;
            _currentUnit = unit;
            _currentUnit.TryGetBotData(out _botData);
        }
    }
}