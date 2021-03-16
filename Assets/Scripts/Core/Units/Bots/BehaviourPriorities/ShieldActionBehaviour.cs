using MageBattle.Core.Units.Bots.BehaviourOperations;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourPriorities
{
    public class ShieldActionBehaviour : IBotBehaviour
    {
        private Unit _currentUnit;
        private BotData _botData;

        private const float _defaultPriorityMultiplier = 0.3f;
        private const float _lowHPMultiplier = 0.5f;
        private const float _patternMultiplier = 0.2f;
        private const int _spellId = 3;

        public IBehaviourOperation operation { get; private set; }
        public bool isAbsoluteChoice { get; private set; } = false;

        public ShieldActionBehaviour()
        {
            operation = new ShieldActionOperation();
        }

        public int GetPriority(Unit unit)
        {
            SetCurrentUnit(unit);
            if (IsAvailableForUnit(unit))
            {
                int fullPriority = BotsDecisionMaker.GetPriorityByMultiplyer(_defaultPriorityMultiplier) + GetLowHPPriority() + GetPatternPriority();
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

        private int GetLowHPPriority()
        {
            return _currentUnit.health <= BotsDecisionMaker.GetUnitLowHPAmount() ? BotsDecisionMaker.GetPriorityByMultiplyer(_lowHPMultiplier) : 0;
        }

        private int GetPatternPriority()
        {
            var pattern = _botData.behaviourPattern;
            int priority = pattern == BotBehaviourPatern.Defender ? BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier)
                : -BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier);
            return priority;
        }

        public bool IsAvailableForUnit(Unit unit)
        {
            SetCurrentUnit(unit);
            return unit.actionsExecutor.CanUseAction(Actions.ActionType.Cast, _spellId);
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