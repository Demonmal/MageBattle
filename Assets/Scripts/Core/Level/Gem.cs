using MageBattle.Core.Units;
using System;
using UnityEngine;

namespace MageBattle.Core.Level
{
    public class Gem : MonoBehaviour
    {
        private Tile _currentTile;

        private const float _offsetY = 0.25f;

        public Action<Unit> onGemGathered;

        public Tile currentTile => _currentTile;

        private void Awake()
        {
            foreach (var unit in UnitsManager.instance.unitsById.Values)
            {
                SubscribeOnUnitMoveEvent(unit);
            }
            UnitsManager.instance.onUnitCreated += OnUnitCreated;
        }

        private void OnUnitCreated(Unit unit)
        {
            SubscribeOnUnitMoveEvent(unit);
        }

        private void SubscribeOnUnitMoveEvent(Unit unit)
        {
            unit.onUnitChangedPosition += OnUnitChangedPosition;
        }

        private void OnUnitChangedPosition(Unit unit, Tile tile)
        {
            if(_currentTile == tile)
            {
                onGemGathered?.Invoke(unit);
            }
        }

        public void Relocate(Tile tile)
        {
            _currentTile = tile;
            transform.position = new Vector3(tile.transform.position.x, _offsetY, tile.transform.position.z);
        }

        private void UnsubscribeOnUnitMoveEvent(Unit unit)
        {
            unit.onUnitChangedPosition -= OnUnitChangedPosition;
        }

        private void OnDestroy()
        {
            UnitsManager.instance.onUnitCreated -= OnUnitCreated;
            foreach (var unit in UnitsManager.instance.unitsById.Values)
            {
                UnsubscribeOnUnitMoveEvent(unit);
            }
        }
    }
}