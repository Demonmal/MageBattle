using MageBattle.Core.Level;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class LevelTest
    {
        private LevelBuilder _levelBuilder;
        private Tile[,] _tiles;

        [SetUp]
        public void Setup()
        {
            _levelBuilder = GameObject.FindObjectOfType<LevelBuilder>();
            _levelBuilder.GenerateLevel();
            _tiles = _levelBuilder.GetAllTiles();
        }

        [Test]
        public void IsLevelSizeCorrect()
        {
            int tilesNumber = _tiles.GetLength(0) * _tiles.GetLength(1);
            Assert.AreEqual(_levelBuilder.sizeX * _levelBuilder.sizeZ, tilesNumber);
        }

        [Test]
        public void IsEveryTileMovable()
        {
            bool result = true;
            Tile tileToCheck;
            for (int x = 0; x < _levelBuilder.sizeX; x++)
            {
                for (int z = 0; z < _levelBuilder.sizeZ; z++)
                {
                    tileToCheck = _levelBuilder.GetTileOnPosition(x, z);
                    if(!tileToCheck.IsBlocked() && !_levelBuilder.CanMoveFromTile(tileToCheck))
                    {
                        result = false;
                        break;
                    }
                }
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void EveryUnblockedTileHasTwoOrMoreWaysToGo()
        {
            bool result = true;
            Queue<Tile> tilesQueue = new Queue<Tile>(_levelBuilder.GetTilesWithoutObstacles());

            if (tilesQueue.Count == 0)
            {
                result = false;
            }
            else
            {
                while (tilesQueue.Count > 0)
                {
                    var tileToCheck = tilesQueue.Dequeue();
                    _levelBuilder.CanMoveFromTile(tileToCheck, out var unblockedTilesCount);
                    if (unblockedTilesCount < 2)
                    {
                        result = false;
                        break;
                    }
                    else if (unblockedTilesCount == 2)
                    {
                        var tilesNearby = _levelBuilder.GetTilesFromFourSights(tileToCheck);
                        int undestroyableTilesAround = 0;
                        foreach (var tileNearby in tilesNearby)
                        {
                            if (tileNearby.IsBlocked() && !_levelBuilder.obstaclesManager.IsObstacleDestroyableOnPlace(tileNearby.x, tileNearby.z))
                            {
                                undestroyableTilesAround++;
                            }
                        }
                        if(undestroyableTilesAround > 1)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            Assert.IsTrue(result);
        }
    }
}
