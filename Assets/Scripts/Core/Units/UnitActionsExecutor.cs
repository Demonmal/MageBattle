using MageBattle.Core.Data;
using MageBattle.Core.Level;
using MageBattle.Core.MatchHandle;
using MageBattle.Core.Units.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units
{
    public class UnitActionsExecutor : MonoBehaviour
    {
        private Unit _unit;
        private bool _isActive;
        private List<ActionType> _actionsAvailable = new List<ActionType>();
        private Queue<UnitsAction> _actionsQueue = new Queue<UnitsAction>();
        private UnitsAction _actionInProgress;
        private bool _inProgress;

        private readonly List<ActionType> _defaultActions = new List<ActionType>() { ActionType.Cast, ActionType.Move };

        public event Action onActionCompleted;

        public bool inProgress => _inProgress;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public void Activate()
        {
            _isActive = true;
            _actionsAvailable.AddRange(_defaultActions);
            _unit.RemoveEffects();
        }

        public bool CanCastAnySpell()
        {
            return _actionsAvailable.Contains(ActionType.Cast) && _unit.mana > 0;
        }

        public bool HasMoveAction()
        {
            return _actionsAvailable.Contains(ActionType.Move);
        }

        public bool Cast(int spellId, Tile tile)
        {
            return UseAction(tile, ActionType.Cast, spellId);
        }

        public bool Move(Tile tile)
        {
            return UseAction(tile, ActionType.Move);
        }

        private bool UseAction(Tile tile, ActionType actionType, int actionId = -1)
        {
            Debug.Log($"UseAction {tile.x}:{tile.z}, {actionType}");
            if (!CanUseAction(actionType, actionId))
                return false;
            if (!ActionRequirements.CanUseAction(actionId, _unit, tile))
                return false;
            _actionsAvailable.Remove(actionType);
            UnitsAction action = UnitActionFactory.CreateAction(actionId, _unit, tile);
            if (_inProgress)
            {
                _actionsQueue.Enqueue(action);
            }
            else
            {
                SetActionInProgress(action);
            }
            return true;
        }

        public bool CanUseAction(ActionType actionType, int actionId = -1)
        {
            bool result = _isActive && _actionsAvailable.Contains(actionType);
            if(actionType == ActionType.Move)
            {
                result &= LevelBuilder.instance.CanMoveFromTile(_unit.currentTile);
            }
            else
            {
                var info = SpellsInfoLoader.spellsInfo[actionId];
                result &= info.manaCost <= _unit.mana;
            }
            Debug.Log($"CanUseAction {actionType}, {actionId} -> {result}");
            return result;
        }

        private void Update()
        {
            if(_isActive && _inProgress)
            {
                _actionInProgress.Update();
            }
        }

        private void SetActionInProgress(UnitsAction action)
        {
            _actionInProgress = action;
            _inProgress = true;
            action.onActionCompleted += OnActionCompleted;
            _actionInProgress.Start();
        }

        private void OnActionCompleted(UnitsAction action)
        {
            action.onActionCompleted -= OnActionCompleted;
            if(_actionsQueue.Count > 0)
            {
                SetActionInProgress(_actionsQueue.Dequeue());
            }
            else
            {
                _inProgress = false;
                _actionInProgress = null;
                CheckIfStepEnd();
            }
            onActionCompleted?.Invoke();
        }

        private void CheckIfStepEnd()
        {
            Debug.Log($"CheckIfStepEnd {CanCastAnySpell()} & {HasMoveAction()}");
            if(!CanCastAnySpell() && !HasMoveAction())
            {
                GameHandler.instance.FinishStep();
            }
        }

        public void Deactivate()
        {
            _isActive = false;
            _actionsQueue.Clear();
            _actionsAvailable.Clear();
        }

        private void OnDestroy()
        {
            if (_actionInProgress != null)
                OnActionCompleted(_actionInProgress);
        }
    }
}