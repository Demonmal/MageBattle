using MageBattle.Core.Data;
using MageBattle.Core.Level;
using MageBattle.Core.Units.Spells;
using UnityEngine;

namespace MageBattle.Core.Units.Actions
{
    public static class ActionRequirements
    {
        public static bool CanUseAction(int actionId, Unit unit, Tile tile)
        {
            bool result = true;
            SpellInfo info;
            bool tileIsNotBlocked;
            bool tileInRange;
            switch (actionId)
            {
                case -1:
                    bool tileIsNotCurrent = tile.id != unit.currentTile.id;
                    result = tileIsNotCurrent && LevelBuilder.instance.pathHelper.CheckIfCanReachTile(unit.currentTile, tile, unit.maxDistanceToMove);
                    break;
                case 1:
                    info = SpellsInfoLoader.spellsInfo[actionId];
                    bool tileIsBlockedByObstacle = tile.IsBlocked();
                    bool canMakeDamage = tile.type == TileType.With_Obstacle ? LevelBuilder.instance.obstaclesManager.IsObstacleDestroyableOnPlace(tile.x, tile.z) : true;
                    tileInRange = LevelBuilder.instance.pathHelper.GetDistanceBetweenTiles(unit.currentTile, tile) == info.radius;
                    result = tileIsBlockedByObstacle && canMakeDamage && tileInRange;
                    break;
                case 2:
                    info = SpellsInfoLoader.spellsInfo[actionId];
                    tileIsNotBlocked = !tile.IsBlocked();
                    tileInRange = LevelBuilder.instance.pathHelper.GetMaxAxisDistanceBetweenTiles(unit.currentTile, tile) <= info.radius;
                    result = tileIsNotBlocked && tileInRange;
                    break;
                case 5:
                case 6:
                    info = SpellsInfoLoader.spellsInfo[actionId];
                    tileInRange = LevelBuilder.instance.pathHelper.GetMaxAxisDistanceBetweenTiles(unit.currentTile, tile) <= info.radius;
                    result = tileInRange;
                    break;
            }
            Debug.Log($"CanUseAction {actionId} -> {result}");
            return result;
        }
    }
}