using MageBattle.Core.Data;
using MageBattle.Core.Enums;
using MageBattle.Core.Level;
using MageBattle.Core.MatchHandle;
using MageBattle.Core.Spawn;
using MageBattle.Core.Units.Bots;
using MageBattle.Utility;
using System;
using UnityEngine;

namespace MageBattle.Core.Units
{
    public class Unit : MonoBehaviour
    {
        private UnitData _data;
        private int _mana;
        private float _health;
        private int _manaToRegenPerRound;
        private int _maxDistanceToMove;
        private bool _isShielded;
        private bool _isInvisible;
        private MeshRenderer _meshRenderer;
        private Fraction _fraction;
        private Tile _currentTile;
        private UnitActionsExecutor _actionsExecutor;
        private bool _isActive;

        private PlayerController _playerController;
        private AIController _aiController;

        private readonly Vector3 _deathPosition = new Vector3(1000, 1000, 1000);

        public UnitData data => _data;
        public int mana => _mana;
        public float health => _health;
        public int manaToRegenPerRound => _manaToRegenPerRound;
        public int maxDistanceToMove => _maxDistanceToMove;
        public bool isInvisible => _isInvisible;
        public Fraction fraction => _fraction;
        public Tile currentTile => _currentTile;
        public UnitActionsExecutor actionsExecutor => _actionsExecutor;

        public Action<Unit, Tile> onUnitChangedPosition;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameHandler.instance.onNextStep += OnNextStep;
            GameHandler.instance.onRoundEnd += OnRoundEnd;
        }

        public void Inititialize(UnitData userData, Fraction fraction)
        {
            _data = userData;
            SetMatchParams();
            _fraction = fraction;
            _actionsExecutor = gameObject.AddComponent(typeof(UnitActionsExecutor)) as UnitActionsExecutor;
            if(_data.isMine)
            {
                _playerController = gameObject.AddComponent(typeof(PlayerController)) as PlayerController;
                _meshRenderer.material.color = ColorsHelper.minePlayerColor;
            }
            else if (_data.isBot)
            {
                _aiController = gameObject.AddComponent(typeof(AIController)) as AIController;
            }
        }

        public void SetMatchParams()
        {
            var matchInfo = GameHandler.instance.matchInfo;
            _mana = matchInfo.defaultUnitsMana;
            _health = matchInfo.defaultUnitsHealth;
            _manaToRegenPerRound = matchInfo.manaToRegenPerRound;
            _maxDistanceToMove = matchInfo.maxDistanceToMove;
        }

        private void OnNextStep(Unit unit)
        {
            if (unit.data.userId == _data.userId)
            {
                Activate();
            }
            else if (_isActive)
            {
                Deactivate();
            }
        }

        private void OnRoundEnd()
        {
            if (_isActive)
            {
                Deactivate();
            }
        }

        private void Activate()
        {
            _isActive = true;
            _actionsExecutor.Activate();
            if(_data.isMine)
            {
                _playerController.Activate();
            }
            else if (data.isBot)
            {
                _aiController.Activate();
            }

            SetMeshActiveColor(true);
        }

        private void Deactivate()
        {
            _isActive = false;
            _actionsExecutor.Deactivate();
            if (_data.isMine)
            {
                _playerController.Deactivate();
            }
            else if (data.isBot)
            {
                _aiController.Deactivate();
            }

            SetMeshActiveColor(false);
        }

        private void SetMeshActiveColor(bool state)
        {
            if (!_data.isMine)
            {
                _meshRenderer.material.color = state ? ColorsHelper.activePlayerColor : ColorsHelper.defaultMeshColor;
            }
        }

        public bool Cast(int spellId, Tile tile)
        {
            Debug.Log($"Cast {spellId}");
            return _actionsExecutor.Cast(spellId, tile);
        }

        public bool Move(Tile tile)
        {
            return _actionsExecutor.Move(tile);
        }

        public void Revive()
        {
            SetMatchParams();
            UnitsManager.instance.OnUnitRevived(this);
        }

        public void ReceiveDamage(DamageSource damageSource)
        {
            if(_isShielded)
            {
                _isShielded = false;
                return;
            }
            float damage = DamageValuesContainer.damageBySource[damageSource];
            _health = _health - damage < 0 ? 0 : _health - damage;
            if(_health == 0)
            {
                Killed();
            }
        }

        public void SetShield()
        {
            _isShielded = true;
        }

        public void SetInvisibility(bool state)
        {
            if (_isInvisible == state)
                return;
            _isInvisible = state;
            Color color = _meshRenderer.material.color;
            if(_isInvisible)
            {
                color.a = _data.isMine ? 0.7f : 0;
            }
            else
            {
                color.a = 1;
            }
            _meshRenderer.material.color = color;
        }

        public void RemoveEffects()
        {
            _isShielded = false;
            SetInvisibility(false);
        }

        private void Killed()
        {
            transform.position = _deathPosition;
            UnitsManager.instance.OnUnitKilled(this);
        }

        public bool IsStucked()
        {
            bool hasMoveAction = _actionsExecutor.HasMoveAction();
            bool canMoveFromTile = LevelBuilder.instance.CanMoveFromTile(_currentTile);
            return hasMoveAction && !canMoveFromTile;
        }

        public bool TryGetBotData(out BotData botData)
        {
            bool result = false;
            botData = null;
            if (data.isBot && BotsCreator.instance.dataById.ContainsKey(data.userId))
            {
                result = true;
                botData = BotsCreator.instance.dataById[data.userId];
            }
            return result;
        }

        public void SetCurrentTile(Tile tile)
        {
            _currentTile?.MarkAsEmpty();
            _currentTile = tile;
            _currentTile.SetType(TileType.With_Player);
            onUnitChangedPosition?.Invoke(this, tile);
        }

        private void UnsubscribeEvets()
        {
            GameHandler.instance.onNextStep -= OnNextStep;
            GameHandler.instance.onRoundEnd -= OnRoundEnd;
        }

        private void OnDestroy()
        {
            UnsubscribeEvets();
        }
    }
}