using MageBattle.Core.Level;
using MageBattle.Core.MatchHandle;
using MageBattle.Core.Units.Bots.BehaviourOperations;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourPriorities
{
    public class MovementBehaviour : IBotBehaviour
    {
        private Unit _currentUnit;
        private BotData _botData;

        private const float _defaultPriorityMultiplier = 0.2f;
        private const float _patternMultiplier = 0.1f;

        public IBehaviourOperation operation { get; private set; }
        public bool isAbsoluteChoice { get; private set; } = false;

        public MovementBehaviour()
        {
            operation = new MovementOperation(this);
        }

        public int GetPriority(Unit unit)
        {
            SetCurrentUnit(unit);
            if(IsAvailableForUnit(unit))
            {
                int fullPriority = BotsDecisionMaker.GetPriorityByMultiplyer(_defaultPriorityMultiplier) + GetPatternPriority();
                return Mathf.Clamp(fullPriority, 0, BotsDecisionMaker.maxPriority);
            }
            else
            {
                return 0;
            }
        }

        public bool CanReachGem(Unit unit)
        {
            float distanceToGem = LevelBuilder.instance.pathHelper.GetDistanceBetweenTiles(unit.currentTile, LevelBuilder.instance.gem.currentTile);
            int maxDistanceToMove = GameHandler.instance.matchInfo.maxDistanceToMove;
            return distanceToGem <= maxDistanceToMove;
        }

        private int GetPatternPriority()
        {
            int result = 0;
            var pattern = _botData.behaviourPattern;
            if (pattern == BotBehaviourPatern.Defender || pattern == BotBehaviourPatern.Gatherer)
            {
                if (IsAnyUnitNearby())
                {
                    result = BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier);
                }
            }
            else if (pattern == BotBehaviourPatern.Agressor)
            {
                bool hasRodAction = _botData.spellsId.Contains(1);
                if (hasRodAction && IsAnyUnitNearby())
                {
                    result = BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier);
                }
            }
            return result;
        }

        private bool IsAnyUnitNearby()
        {
            bool result = false;
            foreach (var unit in UnitsManager.instance.aliveUnits)
            {
                if (unit.data.userId == _currentUnit.data.userId)
                    continue;
                if(LevelBuilder.instance.pathHelper.GetDistanceBetweenTiles(_currentUnit.currentTile, unit.currentTile) <= 2)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool IsAvailableForUnit(Unit unit)
        {
            return unit.actionsExecutor.CanUseAction(Actions.ActionType.Move);
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