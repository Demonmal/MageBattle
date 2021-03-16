using MageBattle.Core.Data;
using MageBattle.Core.Level;
using MageBattle.Core.Units.Bots.BehaviourOperations;
using MageBattle.Core.Units.Spells;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourPriorities
{
    public class BlinkActionBehaviour : IBotBehaviour
    {
        private Unit _currentUnit;
        private BotData _botData;
        private SpellInfo _spellInfo;

        private const float _defaultPriorityMultiplier = 0.3f;
        private const float _playerStuckedMultiplier = 1f;
        private const float _lowHPMultiplier = 0.5f;
        private const float _patternMultiplier = 0.2f;

        private const int _spellId = 2;

        public IBehaviourOperation operation { get; private set; }
        public bool isAbsoluteChoice { get; private set; }

        public BlinkActionBehaviour()
        {
            _spellInfo = SpellsInfoLoader.spellsInfo[_spellId];
            operation = new BlinkActionOperation(this);
        }

        public int GetPriority(Unit unit)
        {
            SetCurrentUnit(unit);
            isAbsoluteChoice = false;
            if (IsAvailableForUnit(unit))
            {
                int fullPriority = BotsDecisionMaker.GetPriorityByMultiplyer(_defaultPriorityMultiplier) + GetPlayerStuckPriority() + GetPatternPriority() + GetLowHPPriority();
                return Mathf.Clamp(fullPriority, 0, BotsDecisionMaker.maxPriority);
            }
            else
            {
                return 0;
            }
        }

        public bool CanReachGem(Unit unit)
        {
            return LevelBuilder.instance.pathHelper.GetMaxAxisDistanceBetweenTiles(unit.currentTile, LevelBuilder.instance.gem.currentTile) <= _spellInfo.radius;
        }

        private int GetPlayerStuckPriority()
        {
            int priority = 0;
            if (_currentUnit.IsStucked())
            {
                priority = BotsDecisionMaker.GetPriorityByMultiplyer(_playerStuckedMultiplier);
                isAbsoluteChoice = true;
            }
            return priority;
        }

        private int GetLowHPPriority()
        {
            return _currentUnit.health <= BotsDecisionMaker.GetUnitLowHPAmount() ? BotsDecisionMaker.GetPriorityByMultiplyer(_lowHPMultiplier) : 0;
        }

        private int GetPatternPriority()
        {
            int priority = 0;
            if (isAbsoluteChoice)
                return priority;
            var pattern = _botData.behaviourPattern;
            priority = pattern == BotBehaviourPatern.Defender || pattern == BotBehaviourPatern.Gatherer ? BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier)
                : -BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier);
            return priority;
        }

        public bool IsAvailableForUnit(Unit unit)
        {
            SetCurrentUnit(unit);
            return unit.actionsExecutor.CanUseAction(Actions.ActionType.Cast, _spellId) && ExistAnyTileToBlink();
        }

        private bool ExistAnyTileToBlink()
        {
            (int, int) xRange = (Mathf.Clamp(_currentUnit.currentTile.x - _spellInfo.radius, 0, LevelBuilder.instance.sizeX - 1), 
                Mathf.Clamp(_currentUnit.currentTile.x + _spellInfo.radius, 0, LevelBuilder.instance.sizeX - 1));
            (int, int) zRange = (Mathf.Clamp(_currentUnit.currentTile.z - _spellInfo.radius, 0, LevelBuilder.instance.sizeZ - 1), 
                Mathf.Clamp(_currentUnit.currentTile.z + _spellInfo.radius, 0, LevelBuilder.instance.sizeZ - 1));
            var tiles = LevelBuilder.instance.GetAllTilesInRanges(xRange, zRange);
            foreach (var tile in tiles)
            {
                if (tile.id == _currentUnit.currentTile.id)
                    continue;
                if (tile.IsBlocked())
                    continue;
                if (LevelBuilder.instance.pathHelper.GetMaxAxisDistanceBetweenTiles(_currentUnit.currentTile, tile) > _spellInfo.radius)
                    continue;
                return true;
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