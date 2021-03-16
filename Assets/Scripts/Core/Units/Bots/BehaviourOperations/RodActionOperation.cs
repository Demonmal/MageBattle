using MageBattle.Core.Level;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourOperations
{
    public class RodActionOperation : IBehaviourOperation
    {
        private const int _spellId = 1;

        public void Execute(Unit unit)
        {
            var tilesNeary = LevelBuilder.instance.GetTilesFromFourSights(unit.currentTile);
            List<Tile> possibleTilesToHit = new List<Tile>();
            foreach (var tileNearby in tilesNeary)
            {
                if (tileNearby.IsBlocked())
                {
                    bool playerOnTile = tileNearby.type == TileType.With_Player;
                    bool destroyableObstacleOnTile = tileNearby.type == TileType.With_Obstacle
                        && LevelBuilder.instance.obstaclesManager.IsObstacleDestroyableOnPlace(tileNearby.x, tileNearby.z);
                    if (playerOnTile || destroyableObstacleOnTile)
                    {
                        possibleTilesToHit.Add(tileNearby);
                    }
                }
            }
            if(possibleTilesToHit.Count > 0)
            {
                Tile tileToHit;
                if (possibleTilesToHit.Count == 0)
                {
                    tileToHit = possibleTilesToHit[0];
                }
                else
                {
                    tileToHit = possibleTilesToHit[Random.Range(0, possibleTilesToHit.Count - 1)];
                }
                unit.actionsExecutor.Cast(_spellId, tileToHit);
            }
        }
    }
}