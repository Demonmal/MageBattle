using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Level
{
    public class ObstaclesManager : MonoBehaviour
    {
        private LevelBuilder _levelBuilder;
        private List<Obstacle> _obstacles = new List<Obstacle>();

        private const string _commonUndestroyableObstacleId = "common_undestroyable";
        private const string _commonDestroyableObstacleId = "common_destroyable";

        public IReadOnlyList<Obstacle> obstacles => _obstacles;

        public void Initialize(LevelBuilder builder)
        {
            _levelBuilder = builder;
            _obstacles.Clear();
        }

        public void CreateDefaultLevelObstacles(int destroyableCount, int undestroyableCount)
        {
            var shuffledTiles = ShuffleArray.Shuffle(_levelBuilder.GetTilesWithoutObstacles());
            Queue<Tile> tilesQueue = new Queue<Tile>(shuffledTiles);
            for (int i = 0; i < destroyableCount; i++)
            {
                if (TryGetSatisfiedTileToSetObstacle(tilesQueue, _commonDestroyableObstacleId, out var tileToSet))
                {
                    CreateObstacle(_commonDestroyableObstacleId, tileToSet);
                }
            }
            for (int i = 0; i < undestroyableCount; i++)
            {
                if(TryGetSatisfiedTileToSetObstacle(tilesQueue, _commonUndestroyableObstacleId, out var tileToSet))
                {
                    CreateObstacle(_commonUndestroyableObstacleId, tileToSet);
                }
            }
        }

        private bool TryGetSatisfiedTileToSetObstacle(Queue<Tile> queue, string id, out Tile tileToTake)
        {
            tileToTake = null;
            if (queue.Count == 0)
                return false;

            while (queue.Count > 0)
            {
                var tileToCheck = queue.Dequeue();
                tileToCheck.SetType(TileType.With_Obstacle);
                bool allTilesHaveTwoWaysToGo = true;
                foreach (var tile in _levelBuilder.GetAllTiles())
                {
                    if(!TileSatisfiesRequirements(tile, tileToCheck))
                    {
                        allTilesHaveTwoWaysToGo = false;
                        break;
                    }
                }
                tileToCheck.MarkAsEmpty();
                if (allTilesHaveTwoWaysToGo)
                {
                    tileToTake = tileToCheck;
                    break;
                }
            }

            return tileToTake != null;

            bool TileSatisfiesRequirements(Tile tile, Tile tileToCheck)
            {
                bool result = true;
                _levelBuilder.CanMoveFromTile(tile, out var unblockedTilesCount);
                if (unblockedTilesCount < 2)
                {
                    result = false;
                }
                else if (unblockedTilesCount == 2)
                {
                    var tilesNearby = _levelBuilder.GetTilesFromFourSights(tile);
                    if (tilesNearby.Contains(tileToCheck) && !_levelBuilder.obstaclesPool.IsObstacleDestroyableById(id))
                    {
                        foreach (var tileNearby in tilesNearby)
                        {
                            if (tileNearby.IsBlocked() && tileNearby != tileToCheck && !IsObstacleDestroyableOnPlace(tileNearby.x, tileNearby.z))
                            {
                                result = false;
                            }
                        }
                    }
                }
                return result;
            }
        }

        public Obstacle CreateObstacle(string id, Tile tile)
        {
            var obstacle = _levelBuilder.obstaclesPool.PopById(id);
            obstacle.Spawn(tile);
            obstacle.transform.SetParent(_levelBuilder.obstaclesHolderObj);
            _obstacles.Add(obstacle);
            return obstacle;
        }

        public bool TryGetObstacleOnPlace(int x, int z, out Obstacle obstacle)
        {
            obstacle = _obstacles.Find(l_obstacle => l_obstacle.tile.x == x && l_obstacle.tile.z == z);
            return obstacle != null;
        }

        public bool IsObstacleExistsOnPlace(int x, int z)
        {
            var result = _obstacles.Find(obstacle => obstacle.tile.x == x && obstacle.tile.z == z);
            return result != null;
        }

        public bool IsObstacleDestroyableOnPlace(int x, int z)
        {
            var result = _obstacles.Find(obstacle => obstacle.tile.x == x && obstacle.tile.z == z);
            return result.destroyable;
        }

        public void OnDestroyedObstacle(Obstacle obstacle)
        {
            _obstacles.Remove(obstacle);
        }
    }
}