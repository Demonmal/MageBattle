using MageBattle.Core.Level;
using MageBattle.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units
{
    public class UnitsManager : SingletonObject<UnitsManager>
    {
        private Dictionary<string, Unit> _unitsById = new Dictionary<string, Unit>();
        private Dictionary<Fraction, Unit> _unitsByFraction = new Dictionary<Fraction, Unit>();
        private List<Unit> _aliveUnits = new List<Unit>();

        public IReadOnlyDictionary<Fraction, Unit> unitsByFraction => _unitsByFraction;
        public IReadOnlyDictionary<string, Unit> unitsById => _unitsById;
        public IReadOnlyList<Unit> aliveUnits => _aliveUnits;

        public event Action<Unit> onUnitCreated;
        public event Action<Unit> onUnitKilled;
        public event Action<Unit> onUnitRevived;

        private void Awake()
        {
            _instance = this;
        }

        public void OnUnitCreated(Unit unit)
        {
            if(!unitsById.ContainsKey(unit.data.userId))
            {
                _unitsById.Add(unit.data.userId, unit);
                _unitsByFraction.Add(unit.fraction, unit);
                _aliveUnits.Add(unit);
                onUnitCreated?.Invoke(unit);
            }
        }

        public void OnUnitKilled(Unit unit)
        {
            if(_aliveUnits.Contains(unit))
            {
                _aliveUnits.Remove(unit);
                onUnitKilled?.Invoke(unit);
            }
        }

        public void OnUnitRevived(Unit unit)
        {
            if (!_aliveUnits.Contains(unit))
            {
                _aliveUnits.Add(unit);
                onUnitRevived?.Invoke(unit);
            }
        }

        public bool TryGetNearestUnitToCurrentUnit(Unit currentUnit, out Unit unit, bool notInvisible = true)
        {
            unit = null;
            if (_aliveUnits.Count == 0 || _aliveUnits.Count == 1)
                return false;
            float minDistance = float.MaxValue;
            Tile currentTile = currentUnit.currentTile;
            foreach (var unitToCheck in UnitsManager.instance.aliveUnits)
            {
                if (unitToCheck.data.userId == currentUnit.data.userId)
                    continue;
                if (notInvisible && unitToCheck.isInvisible)
                    continue;
                float distance = LevelBuilder.instance.pathHelper.GetDistanceBetweenTiles(currentTile, unitToCheck.currentTile);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    unit = unitToCheck;
                }
            }
            return true;
        }
    }
}