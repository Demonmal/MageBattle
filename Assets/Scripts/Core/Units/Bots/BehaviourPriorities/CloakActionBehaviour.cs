using MageBattle.Core.Units.Bots.BehaviourOperations;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourPriorities
{
    public class CloakActionBehaviour : IBotBehaviour
    {
        private Unit _currentUnit;
        private BotData _botData;

        private const float _defaultPriorityMultiplier = 0.3f;
        private const float _patternMultiplier = 0.1f;
        private const int _spellId = 4;

        public IBehaviourOperation operation { get; private set; }
        public bool isAbsoluteChoice { get; private set; }

        public CloakActionBehaviour()
        {
            operation = new CloakActionOperation();
        }

        public int GetPriority(Unit unit)
        {
            SetCurrentUnit(unit);
            if (IsAvailableForUnit(unit))
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
            return false;
        }

        private int GetPatternPriority()
        {
            var pattern = _botData.behaviourPattern;
            int priority = pattern == BotBehaviourPatern.Defender || pattern == BotBehaviourPatern.Gatherer ? BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier)
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