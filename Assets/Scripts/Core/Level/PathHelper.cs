using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Level
{
    public class PathHelper
    {
        private LevelBuilder _levelBuilder;
        private List<int> _passedTiles;

        public PathHelper(LevelBuilder levelBuilder)
        {
            _levelBuilder = levelBuilder;
            _passedTiles = new List<int>();
        }

        public bool IsPathExistFromTo(Tile from, Tile to, bool ignoreUnitOnLastTile = false)
        {
            Tile current = from;
            List<Tile> satisfyedTiles = new List<Tile>();
            while (current.id != to.id)
            {
                var tilesNearby = _levelBuilder.GetTilesFromFourSights(current);
                satisfyedTiles.Clear();
                foreach (var tileNearby in tilesNearby)
                {
                    if (_passedTiles.Contains(tileNearby.id))
                        continue;
                    if (tileNearby.IsBlocked())
                    {
                        if (!ignoreUnitOnLastTile || tileNearby.type != TileType.With_Player || tileNearby.id != to.id)
                            continue;
                    }
                    if (tileNearby.id == from.id)
                        continue;
                    satisfyedTiles.Add(tileNearby);
                }
                if (satisfyedTiles.Count == 0)
                {
                    return false;
                }

                _passedTiles.Add(current.id);
                current = GetNearestTileToTarget(satisfyedTiles, to);
            }
            _passedTiles.Clear();
            return true;
        }

        public bool TryGetPathFromTo(Tile from, Tile to, out List<Tile> path, bool ignoreUnitOnLastTile = false)
        {
            path = new List<Tile>();
            Tile current = from;
            List<Tile> satisfyedTiles = new List<Tile>();
            while (current.id != to.id)
            {
                var tilesNearby = _levelBuilder.GetTilesFromFourSights(current);
                satisfyedTiles.Clear();
                foreach (var tileNearby in tilesNearby)
                {
                    if (_passedTiles.Contains(tileNearby.id))
                        continue;
                    if (tileNearby.IsBlocked())
                    {
                        if(!ignoreUnitOnLastTile || tileNearby.type != TileType.With_Player || tileNearby.id != to.id)
                            continue;
                    }  
                    if (tileNearby.id == from.id)
                        continue;
                    satisfyedTiles.Add(tileNearby);
                }
                if (satisfyedTiles.Count == 0)
                {
                    Debug.Log($"Failed on tile {current.x}:{current.z}");
                    return false;
                }
                
                _passedTiles.Add(current.id);
                current = GetNearestTileToTarget(satisfyedTiles, to);
                if(!current.IsBlocked())
                {
                    path.Add(current);
                }
            }
            _passedTiles.Clear();
            return true;
        }

        public bool TryGetFastestPathFromTo(Tile from, Tile to, out List<Tile> path)
        {
            path = new List<Tile>();
            Tile current = from;
            List<Tile> satisfyedTiles = new List<Tile>();
            while (current.id != to.id)
            {
                var tilesNearby = _levelBuilder.GetTilesFromFourSights(current);
                satisfyedTiles.Clear();
                foreach (var tileNearby in tilesNearby)
                {
                    if (_passedTiles.Contains(tileNearby.id))
                        continue;
                    if (tileNearby.id == from.id)
                        continue;
                    satisfyedTiles.Add(tileNearby);
                }
                if (satisfyedTiles.Count == 0)
                {
                    return false;
                }

                _passedTiles.Add(current.id);
                current = GetNearestTileToTarget(satisfyedTiles, to);
                path.Add(current);
            }
            _passedTiles.Clear();
            return true;
        }

        private Tile GetNearestTileToTarget(List<Tile> tiles, Tile target)
        {
            float minDistance = float.MaxValue;
            Tile result = tiles[0];
            foreach (var tile in tiles)
            {
                float distance = GetDistanceBetweenTiles(tile, target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = tile;
                }
            }
            return result;
        }

        public float GetDistanceBetweenTiles(Tile current, Tile to)
        {
            int distanceX = Mathf.Abs(to.x - current.x);
            int distanceZ = Mathf.Abs(to.z - current.z);
            return distanceX + distanceZ;
        }

        public bool CheckIfCanReachTile(Tile current, Tile to, float maxDistanceToMove)
        {
            bool tileIsNotBlocked = !to.IsBlocked();
            bool pathExist = TryGetPathFromTo(current, to, out var path);
            bool distanceSatisfies = pathExist ? path.Count <= maxDistanceToMove : false;
            return tileIsNotBlocked && pathExist && distanceSatisfies;
        }

        public Tile GetFarthestTileToTarget(Tile current, Tile to, int maxDistanceToMove)
        {
            Tile tile = current;
            if(TryGetPathFromTo(current, to, out var path))
            {
                tile = path[maxDistanceToMove - 1];
            }
            return tile;
        }

        public float GetMaxAxisDistanceBetweenTiles(Tile current, Tile to)
        {
            int distanceX = Mathf.Abs(to.x - current.x);
            int distanceZ = Mathf.Abs(to.z - current.z);
            return distanceX > distanceZ ? distanceX : distanceZ;
        }
    }
}