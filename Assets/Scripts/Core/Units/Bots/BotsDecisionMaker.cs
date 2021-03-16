using MageBattle.Core.MatchHandle;
using MageBattle.Core.Units.Bots.BehaviourPriorities;
using MageBattle.Utility;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MageBattle.Core.Units.Bots
{
    public class BotsDecisionMaker
    {
        private Dictionary<BotBehaviourType, IBotBehaviour> _behaviours = new Dictionary<BotBehaviourType, IBotBehaviour>();
        private List<IBotBehaviour> _behavioursToReachGem = new List<IBotBehaviour>(2);
        private Dictionary<IBotBehaviour, float> _behavioursToCheck = new Dictionary<IBotBehaviour, float>();
        private List<IBotBehaviour> _absoluteBehaviours = new List<IBotBehaviour>(2);
        private static float _maxUnitHealth;
        private static readonly float _lowHPPercent = 0.3f;
        private const float _probabilityToMoveBeforeActions = 50;

        public static readonly int maxPriority = 10;

        public BotsDecisionMaker()
        {
            _maxUnitHealth = GameHandler.instance.matchInfo.defaultUnitsHealth;
            var behaviourTypes = (BotBehaviourType[])typeof(BotBehaviourType).GetEnumValues();
            foreach (var type in behaviourTypes)
            {
                CreateBehaviour(type);
            }
        }

        private void CreateBehaviour(BotBehaviourType type)
        {
            IBotBehaviour behaviour = null;
            switch (type)
            {
                case BotBehaviourType.Movement:
                    behaviour = new MovementBehaviour();
                    break;
                case BotBehaviourType.Rod_action:
                    behaviour = new RodActionBehaviour();
                    break;
                case BotBehaviourType.Blink_action:
                    behaviour = new BlinkActionBehaviour();
                    break;
                case BotBehaviourType.Shield_action:
                    behaviour = new ShieldActionBehaviour();
                    break;
                case BotBehaviourType.Cloak_action:
                    behaviour = new CloakActionBehaviour();
                    break;
                case BotBehaviourType.Trap_action:
                    behaviour = new TrapActionBehaviour();
                    break;
                case BotBehaviourType.Meteor_action:
                    behaviour = new MeteorActionBehaviour();
                    break;
            }
            _behaviours.Add(type, behaviour);
        }

        public void Cheat_CommitBehaviourByType(Unit unit, BotBehaviourType type)
        {
            DebugUtility.Log(Color.yellow, $"[AI] Cheat_CommitBehaviourByType {type}");
            IBotBehaviour behaviourToCommit = _behaviours[type];
            behaviourToCommit.GetPriority(unit);
            behaviourToCommit.operation.Execute(unit);
        }

        public void ChooseAndCommitBestBehaviourForUnit(Unit unit)
        {
            IBotBehaviour behaviourToCommit;
            if (TryGetBehaviourWhichCanReachTheGem(unit, out behaviourToCommit))
            {
                DebugUtility.Log(Color.yellow, $"[AI] ExecuteBehaviourWhichCanReachTheGem {behaviourToCommit.GetType().Name}");
                behaviourToCommit.operation.Execute(unit);

                if (!(behaviourToCommit is MovementBehaviour))
                {
                    _behaviours[BotBehaviourType.Movement].operation.Execute(unit);
                }
                else
                {
                    behaviourToCommit = GetBehaviourToCommitExceptMovement(unit);
                    behaviourToCommit?.operation.Execute(unit);
                }
            }
            else
            {
                bool toMoveFirst = Random.Range(0, 100) <= _probabilityToMoveBeforeActions;
                DebugUtility.Log(Color.yellow, $"[AI] toMoveFirst {toMoveFirst}");
                if (toMoveFirst)
                {
                    _behaviours[BotBehaviourType.Movement].operation.Execute(unit);
                    behaviourToCommit = GetBehaviourToCommitExceptMovement(unit);
                    behaviourToCommit.operation.Execute(unit);
                }
                else
                {
                    behaviourToCommit = GetBehaviourToCommitExceptMovement(unit);
                    behaviourToCommit?.operation.Execute(unit);
                    _behaviours[BotBehaviourType.Movement].operation.Execute(unit);
                }
            }
        }

        private bool TryGetBehaviourWhichCanReachTheGem(Unit unit, out IBotBehaviour behaviourToCommit)
        {
            behaviourToCommit = null;
            _behavioursToReachGem.Clear();
            foreach (var behaviour in _behaviours.Values)
            {
                if (behaviour.CanReachGem(unit) && behaviour.IsAvailableForUnit(unit))
                {
                    _behavioursToReachGem.Add(behaviour);
                }
            }
            if (_behavioursToReachGem.Count > 0)
            {
                if (_behavioursToReachGem.Count == 1)
                {
                    behaviourToCommit = _behavioursToReachGem[0];
                }
                else
                {
                    behaviourToCommit = _behavioursToReachGem[Random.Range(0, _behavioursToReachGem.Count - 1)];
                }
                DebugUtility.Log(Color.yellow, $"[AI] BehaviourWhichCanReachTheGem exist");
                return true;
            }
            return false;
        }

        private IBotBehaviour GetBehaviourToCommitExceptMovement(Unit unit)
        {
            IBotBehaviour behaviourToCommit;
            _absoluteBehaviours.Clear();
            _behavioursToCheck.Clear();
            float allPriorityRange = GetAllPriorityRangeAndFillCollections(unit);
            if (_absoluteBehaviours.Count > 0)
            {
                behaviourToCommit = GetAbsoluteBehaviourToCommit();
            }
            else
            {
                behaviourToCommit = GetBehaviourToCommitByRandomWithPriorities(allPriorityRange);
            }
            return behaviourToCommit;
        }

        private float GetAllPriorityRangeAndFillCollections(Unit unit)
        {
            float allPriorityRange = 0;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"[AI] GetAllPriorityRangeAndFillCollections:");
            foreach (var behaviour in _behaviours.Values)
            {
                if (behaviour is MovementBehaviour)
                    continue;
                float priority = behaviour.GetPriority(unit);
                if (priority == 0)
                    continue;
                stringBuilder.Append($"{behaviour.GetType().Name} -> {priority} & {behaviour.isAbsoluteChoice},");
                _behavioursToCheck.Add(behaviour, priority);
                allPriorityRange += priority;
                if (behaviour.isAbsoluteChoice)
                    _absoluteBehaviours.Add(behaviour);
            }
            stringBuilder.Append($"allPriorityRange = {allPriorityRange}");
            DebugUtility.Log(Color.yellow, stringBuilder.ToString());
            return allPriorityRange;
        }

        private IBotBehaviour GetAbsoluteBehaviourToCommit()
        {
            IBotBehaviour behaviourToCommit;
            if (_absoluteBehaviours.Count == 1)
            {
                behaviourToCommit = _absoluteBehaviours[0];
            }
            else
            {
                behaviourToCommit = _absoluteBehaviours[Random.Range(0, _absoluteBehaviours.Count - 1)];
            }
            DebugUtility.Log(Color.yellow, $"[AI] GetAbsoluteBehaviourToCommit {behaviourToCommit.GetType().Name}");
            return behaviourToCommit;
        }

        private IBotBehaviour GetBehaviourToCommitByRandomWithPriorities(float allPriorityRange)
        {
            IBotBehaviour behaviourToCommit = null;
            float priorityIndexToTake = Random.Range(1, allPriorityRange);
            float lastPriorityIndex = 0;
            foreach (var behaviour in _behavioursToCheck)
            {
                if (priorityIndexToTake > lastPriorityIndex && priorityIndexToTake <= lastPriorityIndex + behaviour.Value)
                {
                    behaviourToCommit = behaviour.Key;
                    break;
                }
                else
                {
                    lastPriorityIndex += behaviour.Value;
                }
            }
            DebugUtility.Log(Color.yellow, $"[AI] GetBehaviourToCommitByRandomWithPriorities {behaviourToCommit.GetType().Name}, priorityIndexToTake {priorityIndexToTake}");
            return behaviourToCommit;
        }

        public static int GetPriorityByMultiplyer(float multiplier)
        {
            return (int)(multiplier * maxPriority);
        }

        public static float GetUnitLowHPAmount()
        {
            return _maxUnitHealth * _lowHPPercent;
        }
    }
}