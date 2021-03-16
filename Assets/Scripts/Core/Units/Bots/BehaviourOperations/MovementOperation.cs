using MageBattle.Core.Level;
using MageBattle.Core.Units.Bots.BehaviourPriorities;
using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourOperations
{
    public class MovementOperation : IBehaviourOperation
    {
        private IBotBehaviour _botBehaviour;
        private Unit _currentUnit;
        private BotData _botData;
        private LevelBuilder _levelBuilder;
        private PathHelper _pathHelper;

        private const float _defenderProbabilityToMoveToGem = 0.7f;
        private const float _agressorProbabilityToMoveToGem = 0.3f;
        private const int _tilesBetweenAgressorAndEnemyToHaveWithLowMana = 1;
        private const int _tilesBetweenAgressorAndEnemyToHaveWithHighMana = 3;
        private const int _lowManaThreshold = 1;        

        public MovementOperation(IBotBehaviour botBehaviour)
        {
            _botBehaviour = botBehaviour;
            _levelBuilder = LevelBuilder.instance;
            _pathHelper = LevelBuilder.instance.pathHelper;
        }

        public void Execute(Unit unit)
        {
            SetCurrentUnit(unit);
            Tile tile = GetTileToMove();
            if (tile.id != _currentUnit.currentTile.id)
            {
                unit.actionsExecutor.Move(tile);
            }
            DebugUtility.Log(Color.yellow, $"[AI] Execute movement {tile.x}:{tile.z}, same tile {tile.id == _currentUnit.currentTile.id}");
        }

        private Tile GetTileToMove()
        {
            Tile tile = _currentUnit.currentTile;
            if(_botBehaviour.CanReachGem(_currentUnit))
            {
                tile = LevelBuilder.instance.gem.currentTile;
            }
            else
            {
                var pattern = _botData.behaviourPattern;
                switch (pattern)
                {
                    case BotBehaviourPatern.Defender:
                        tile = GetTileForDefender();
                        break;
                    case BotBehaviourPatern.Agressor:
                        tile = GetTileForAgressor();
                        break;
                    case BotBehaviourPatern.Gatherer:
                        tile = GetTileForGatherer();
                        break;
                }
            }
            return tile;
        }

        private Tile GetTileForDefender()
        {
            bool moveToGem = Random.Range(0, 100) <= _defenderProbabilityToMoveToGem * 100;
            DebugUtility.Log(Color.yellow, $"[AI] GetTileForDefender, moveToGem {moveToGem}");
            if (moveToGem)
            {
                return GetFarthestTileToGem();
            }
            else
            {
                List<Tile> tiles = _levelBuilder.GetAllReachableTiles(_currentUnit.currentTile, _currentUnit.maxDistanceToMove);
                Tile gemTile = _levelBuilder.gem.currentTile;
                int tileIndexToTake = 0;
                float tileCoef = float.MinValue;
                float tempCoef;
                for (int i = 0; i < tiles.Count; i++)
                {
                    tempCoef = 0;
                    if (!_pathHelper.IsPathExistFromTo(_currentUnit.currentTile, tiles[i]))
                        continue;
                    foreach (var unit in UnitsManager.instance.aliveUnits)
                    {
                        if (unit.data.userId == _currentUnit.data.userId)
                            continue;
                        tempCoef += _pathHelper.GetDistanceBetweenTiles(tiles[i], unit.currentTile);
                    }
                    tempCoef -= _pathHelper.GetDistanceBetweenTiles(tiles[i], gemTile);
                    if(tempCoef < tileCoef)
                    {
                        tileCoef = tempCoef;
                        tileIndexToTake = i;
                    }
                }
                return tiles[tileIndexToTake];
            }
        }

        private Tile GetTileForAgressor()
        {
            bool moveToGem = Random.Range(0, 100) <= _agressorProbabilityToMoveToGem * 100;
            Tile tileToGo = _currentUnit.currentTile; 
            if (moveToGem)
            {
                tileToGo = GetFarthestTileToGem();
            }
            else
            {
                bool isLowMana = _currentUnit.mana <= _lowManaThreshold;
                int desiredDistanceBetweenUnits = isLowMana ? _tilesBetweenAgressorAndEnemyToHaveWithLowMana : _tilesBetweenAgressorAndEnemyToHaveWithHighMana;
                if (UnitsManager.instance.TryGetNearestUnitToCurrentUnit(_currentUnit, out var unitToCome))
                {
                    if (_pathHelper.TryGetPathFromTo(_currentUnit.currentTile, unitToCome.currentTile, out var path, true))
                    {
                        if (path.Count > desiredDistanceBetweenUnits)
                        {
                            var tempTile = path[path.Count - desiredDistanceBetweenUnits];
                            if(_pathHelper.GetDistanceBetweenTiles(_currentUnit.currentTile, tempTile) <= _currentUnit.maxDistanceToMove)
                            {
                                tileToGo = tempTile;
                            }
                            else
                            {
                                tileToGo = path[_currentUnit.maxDistanceToMove - 1];
                            }
                        }
                    }
                }
            }
            return tileToGo;
        }

        private Tile GetTileForGatherer()
        {
            return GetFarthestTileToGem();
        }

        private Tile GetFarthestTileToGem()
        {
            return _pathHelper.GetFarthestTileToTarget(_currentUnit.currentTile, _levelBuilder.gem.currentTile, _currentUnit.maxDistanceToMove);
        }

        private void SetCurrentUnit(Unit unit)
        {
            if (_currentUnit == unit)
                return;
            _currentUnit = unit;
            _currentUnit.TryGetBotData(out _botData);
        }
    }
}