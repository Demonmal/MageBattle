using MageBattle.Core.Level;
using MageBattle.Core.Units;
using MageBattle.Profile;
using MageBattle.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Spawn
{
    public class UnitSpawner : SingletonObject<UnitSpawner>
    {
        private List<Tile> _spawnTiles = new List<Tile>();
        private LevelBuilder _levelBuilder;
        private Fraction _fractionToSet = Fraction.Fraction0;

        private const string _unitsPrefabPath = "Prefabs/Unit";

        public IReadOnlyList<Tile> spawnTiles => _spawnTiles;

        private void Awake()
        {
            _instance = this;
            UnitsDataManager.instance.onAddedData += OnAddedUnitData;
        }

        private void OnAddedUnitData(UnitData data)
        {
            CreateUnit(data);
        }

        public void CreateUnit(UnitData data)
        {
            var unitsPrefab = Resources.Load<Unit>(_unitsPrefabPath);
            Unit unit = Instantiate<Unit>(unitsPrefab, UnitsManager.instance.transform);
            Fraction fraction = _fractionToSet;
            unit.Inititialize(data, fraction);
            Tile tileToSpawn = _spawnTiles[(int)_fractionToSet];
            SetUnitPositionOnTile(tileToSpawn, unit);
            UnitsManager.instance.OnUnitCreated(unit);
            _fractionToSet = (_fractionToSet + 1);
        }

        public void RespawnUnit(Unit unit)
        {
            List<Tile> satysfyingTiles = _spawnTiles.FindAll(tile => !tile.IsBlocked());
            Tile tileToRespawn = satysfyingTiles[UnityEngine.Random.Range(0, satysfyingTiles.Count)];
            SetUnitPositionOnTile(tileToRespawn, unit);
            unit.Revive();
        }

        public void AssignSpawnPoints()
        {
            _spawnTiles.Clear();
            _levelBuilder = LevelBuilder.instance;
            (int, int) xRange;
            (int, int) zRange;
            foreach (var key in Enum.GetValues(typeof(Fraction)))
            {
                var fraction = (Fraction)key;
                switch (fraction)
                {
                    case Fraction.Fraction0:
                        xRange = (0, Mathf.FloorToInt(_levelBuilder.sizeX / 2) - 1);
                        zRange = (0, Mathf.FloorToInt(_levelBuilder.sizeZ / 2) - 1);
                        SetTileByFractionInRanges(xRange, zRange);
                        break;
                    case Fraction.Fraction1:
                        xRange = (Mathf.CeilToInt(_levelBuilder.sizeX / 2), _levelBuilder.sizeX - 1);
                        zRange = (0, Mathf.FloorToInt(_levelBuilder.sizeZ / 2) - 1);
                        SetTileByFractionInRanges(xRange, zRange);
                        break;
                    case Fraction.Fraction2:
                        xRange = (0, Mathf.FloorToInt(_levelBuilder.sizeX / 2) - 1);
                        zRange = (Mathf.CeilToInt(_levelBuilder.sizeZ / 2), _levelBuilder.sizeZ - 1);
                        SetTileByFractionInRanges(xRange, zRange);
                        break;
                    case Fraction.Fraction3:
                        xRange = (Mathf.CeilToInt(_levelBuilder.sizeX / 2), _levelBuilder.sizeX - 1);
                        zRange = (Mathf.CeilToInt(_levelBuilder.sizeZ / 2), _levelBuilder.sizeZ - 1);
                        SetTileByFractionInRanges(xRange, zRange);
                        break;
                }
            }
        }

        private void SetUnitPositionOnTile(Tile tile, Unit unit)
        {
            Vector3 position = new Vector3(tile.transform.position.x, 0.7f, tile.transform.position.z);
            unit.transform.position = position;
            unit.SetCurrentTile(tile);
        }

        private void SetTileByFractionInRanges((int, int) xRange, (int, int) zRange)
        {
            var nonBlockedTiles = GetNonblockedTilesInRanges(xRange, zRange);
            var tile = nonBlockedTiles[UnityEngine.Random.Range(0, nonBlockedTiles.Count-1)];
            tile.MarkAsSpawnZone();
            _spawnTiles.Add(tile);
        }

        private List<Tile> GetNonblockedTilesInRanges((int, int) xRange, (int, int) zRange)
        {
            List<Tile> result = new List<Tile>();
            for (int x = xRange.Item1; x < xRange.Item2; x++)
            {
                for (int z = zRange.Item1; z < zRange.Item2; z++)
                {
                    Tile tile = _levelBuilder.GetTileOnPosition(x,z);
                    if(!tile.IsBlocked())
                    {
                        result.Add(tile);
                    }
                }
            }
            return result;
        }

        private void OnDestroy()
        {
            UnitsDataManager.instance.onAddedData -= OnAddedUnitData;
        }
    }
}