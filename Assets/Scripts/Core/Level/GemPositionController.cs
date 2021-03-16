using UnityEngine;

namespace MageBattle.Core.Level
{
    public class GemPositionGiver
    {
        private LevelBuilder _levelBuilder;

        public GemPositionGiver(LevelBuilder levelBuilder)
        {
            _levelBuilder = levelBuilder;
        }

        public Tile GetNewGemTile()
        {
            var tiles = _levelBuilder.GetTilesWithoutObstacles().FindAll(tile => tile.type != TileType.SpawnPoint);
            Tile tileToSetGem = tiles[Random.Range(0, tiles.Count - 1)];
            return tileToSetGem;
        }
    }
}