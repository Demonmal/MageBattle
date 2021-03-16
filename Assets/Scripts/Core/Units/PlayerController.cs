using MageBattle.Core.Data;
using MageBattle.Core.Enums;
using MageBattle.Core.Level;
using MageBattle.Core.MatchHandle;
using MageBattle.Core.Units.Spells;
using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units
{
    public class PlayerController : MonoBehaviour
    {
        private Unit _unit;
        private bool _isActive;
        private int _castSpellId;
        private bool _spellPreparing;

        private IReadOnlyDictionary<KeyCode, KeyBindingType> _keysBindings = new Dictionary<KeyCode, KeyBindingType>()
        {
            {KeyCode.Alpha1, KeyBindingType.Spell1},
            {KeyCode.Alpha2, KeyBindingType.Spell2},
            {KeyCode.Alpha3, KeyBindingType.Spell3},
            {KeyCode.Alpha4, KeyBindingType.Spell4},
            {KeyCode.Escape, KeyBindingType.DiscardSpell},
            {KeyCode.E, KeyBindingType.EndStep}
        };

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        private void OnRoundEnd()
        {
            if (_isActive)
            {
                Deactivate();
            }
        }

        public void Activate()
        {
            Debug.Log($"On my player step");
            _isActive = true;
        }

        public void Deactivate()
        {
            _isActive = false;
            _spellPreparing = false;
        }

        private void Update()
        {
            if (!_isActive || _unit.actionsExecutor.inProgress)
                return;
            if(Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, LayerMaskHelper.instance.groundMask))
                {
                    Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                    if (tile)
                    {
                        OnTileClicked(tile);
                    }
                }
            }
            foreach (var keyCode in _keysBindings.Keys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    OnKeyPressed(keyCode);
                }
            }
        }

        private void OnTileClicked(Tile tile)
        {
            Debug.Log($"OnTileClicked {tile.x}:{tile.z}, _spellPreparing {_spellPreparing}");
            if (!_spellPreparing)
            {
                _unit.Move(tile);
            }
            else
            {
                if(_unit.Cast(_castSpellId, tile))
                {
                    _spellPreparing = false;
                }
            }
        }

        private void OnKeyPressed(KeyCode keyCode)
        {
            KeyBindingType type = _keysBindings[keyCode];
            switch (type)
            {
                case KeyBindingType.Spell1:
                case KeyBindingType.Spell2:
                case KeyBindingType.Spell3:
                case KeyBindingType.Spell4:
                    StartCastSpell((int)type);
                    break;
                case KeyBindingType.DiscardSpell:
                    if(_spellPreparing)
                    {
                        _spellPreparing = false;
                    }
                    break;
                case KeyBindingType.EndStep:
                    GameHandler.instance.FinishStep();
                    break;
            }
        }

        private void StartCastSpell(int spellKey)
        {
            if (_spellPreparing || !_unit.actionsExecutor.CanCastAnySpell())
                return;
            if(spellKey <_unit.data.spellsId.Count)
            {
                int spellId = _unit.data.spellsId[spellKey];
                SpellInfo spellInfo = SpellsInfoLoader.spellsInfo[spellId];
                if(spellInfo.radius == 0)
                {
                    _unit.Cast(spellId, _unit.currentTile);
                }
                else
                {
                    _spellPreparing = true;
                    _castSpellId = spellId;
                }
            }
        }
    }
}