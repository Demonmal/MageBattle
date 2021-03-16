using MageBattle.Core.MatchHandle;
using MageBattle.Core.Units.Bots;
using System;
using UnityEngine;

namespace MageBattle.Core.Units
{
    public class AIController : MonoBehaviour
    {
        private Unit _unit;
        private bool _active;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _unit.actionsExecutor.onActionCompleted += OnActionCompleted;
        }

        public void Activate()
        {
            _active = true;
            PerformAnAction();
        }

        public void Deactivate()
        {
            _active = false;
        }

        private void Update()
        {
            if(_active)
            {
                if (Input.GetKeyDown(KeyCode.Keypad0))
                {
                    GameHandler.instance.botsDecisionMaker.Cheat_CommitBehaviourByType(_unit, BotBehaviourType.Movement);
                }
                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    GameHandler.instance.botsDecisionMaker.Cheat_CommitBehaviourByType(_unit, BotBehaviourType.Rod_action);
                }
                if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    GameHandler.instance.botsDecisionMaker.Cheat_CommitBehaviourByType(_unit, BotBehaviourType.Blink_action);
                }
                if (Input.GetKeyDown(KeyCode.Keypad3))
                {
                    GameHandler.instance.botsDecisionMaker.Cheat_CommitBehaviourByType(_unit, BotBehaviourType.Shield_action);
                }
                if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    GameHandler.instance.botsDecisionMaker.Cheat_CommitBehaviourByType(_unit, BotBehaviourType.Cloak_action);
                }
                if (Input.GetKeyDown(KeyCode.Keypad5))
                {
                    GameHandler.instance.botsDecisionMaker.Cheat_CommitBehaviourByType(_unit, BotBehaviourType.Trap_action);
                }
                if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    GameHandler.instance.botsDecisionMaker.Cheat_CommitBehaviourByType(_unit, BotBehaviourType.Meteor_action);
                }
            }

        }

        private void PerformAnAction()
        {
            GameHandler.instance.botsDecisionMaker.ChooseAndCommitBestBehaviourForUnit(_unit);
            CheckIfExecutorInProgress();
        }

        private void OnActionCompleted()
        {
            CheckIfExecutorInProgress();
        }

        private void CheckIfExecutorInProgress()
        {
            if (!_unit.actionsExecutor.inProgress)
            {
                GameHandler.instance.FinishStep();
            }
        }

        private void UnsubscribeEvents()
        {
            _unit.actionsExecutor.onActionCompleted -= OnActionCompleted;
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }
    }
}