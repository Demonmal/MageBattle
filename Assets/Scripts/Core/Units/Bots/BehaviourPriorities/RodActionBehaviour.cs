using MageBattle.Core.Level;
using MageBattle.Core.MatchHandle;
using MageBattle.Core.Units.Bots.BehaviourOperations;
using UnityEngine;
namespace MageBattle.Core.Units.Bots.BehaviourPriorities
{
    public class RodActionBehaviour : IBotBehaviour
    {
        private Unit _currentUnit;
        private BotData _botData;

        private const float _defaultPriorityMultiplier = 0.3f;
        private const float _playerStuckedMultiplier = 1f;
        private const float _canReachGemButPathIsBlockedMultiplier = 1f;
        private const float _patternMultiplier = 0.2f;

        public IBehaviourOperation operation { get; private set; }
        public bool isAbsoluteChoice { get; private set; }

        public RodActionBehaviour()
        {
            operation = new RodActionOperation();
        }

        public int GetPriority(Unit unit)
        {
            SetCurrentUnit(unit);
            isAbsoluteChoice = false;
            if (IsAvailableForUnit(unit))
            {
                int fullPriority = BotsDecisionMaker.GetPriorityByMultiplyer(_defaultPriorityMultiplier) + GetPlayerStuckPriority() +
                    GetCanReachGemButPathIsBlockedPriority() + GetPatternPriority();
                return Mathf.Clamp(fullPriority, 0, BotsDecisionMaker.maxPriority);
            }
            else
            {
                return 0;
            }
        }

        public bool CanReachGem(Unit unit)
        {
            return false;
        }

        private int GetPlayerStuckPriority()
        {
            int priority = 0;
            if(_currentUnit.IsStucked())
            {
                priority = BotsDecisionMaker.GetPriorityByMultiplyer(_playerStuckedMultiplier);
                isAbsoluteChoice = true;
            }
            return priority;
        }

        private int GetCanReachGemButPathIsBlockedPriority()
        {
            var priority = 0;
            if (LevelBuilder.instance.pathHelper.TryGetFastestPathFromTo(_currentUnit.currentTile, LevelBuilder.instance.gem.currentTile, out var path))
            {
                if(path.Count > GameHandler.instance.matchInfo.maxDistanceToMove)
                {
                    return priority;
                }
                int blockedTilesCount = 0;
                Tile blockingTile = null;
                foreach (var tile in path)
                {
                    if(tile.IsBlocked())
                    {
                        blockedTilesCount++;
                        if (blockedTilesCount > 1)
                        {
                            return priority;
                        }
                        blockingTile = tile;
                    }
                }
                var pattern = _botData.behaviourPattern;
                if(pattern == BotBehaviourPatern.Agressor || pattern == BotBehaviourPatern.Gatherer)
                {
                    if(blockingTile.type != TileType.With_Obstacle || LevelBuilder.instance.obstaclesManager.IsObstacleDestroyableOnPlace(blockingTile.x, blockingTile.z))
                    {
                        priority = BotsDecisionMaker.GetPriorityByMultiplyer(_canReachGemButPathIsBlockedMultiplier);
                        isAbsoluteChoice = true;
                    }
                }
                else if (pattern == BotBehaviourPatern.Defender)
                {
                    if (blockingTile.type == TileType.With_Obstacle && LevelBuilder.instance.obstaclesManager.IsObstacleDestroyableOnPlace(blockingTile.x, blockingTile.z))
                    {
                        priority = BotsDecisionMaker.GetPriorityByMultiplyer(_canReachGemButPathIsBlockedMultiplier);
                        isAbsoluteChoice = true;
                    }
                }
            }

            return priority;
        }

        private int GetPatternPriority()
        {
            int priority = 0;
            if (isAbsoluteChoice)
                return priority;
            var pattern = _botData.behaviourPattern;
            priority = pattern == BotBehaviourPatern.Agressor || pattern == BotBehaviourPatern.Gatherer ? BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier) 
                : -BotsDecisionMaker.GetPriorityByMultiplyer(_patternMultiplier);
            return priority;
        }

        public bool IsAvailableForUnit(Unit unit)
        {
            SetCurrentUnit(unit);
            return unit.actionsExecutor.CanUseAction(Actions.ActionType.Cast, 1) && IsAnyReachableObjectsToCastExist();
        }

        private bool IsAnyReachableObjectsToCastExist()
        {
            bool canMove = _currentUnit.actionsExecutor.CanUseAction(Actions.ActionType.Move);
            bool result = CanHitAnythingFromTile(_currentUnit.currentTile);
            if (!result && canMove)
            {
                var maxDistanceToMove = GameHandler.instance.matchInfo.maxDistanceToMove;
                var reachableTiles = LevelBuilder.instance.GetAllReachableTiles(_currentUnit.currentTile, maxDistanceToMove);
                foreach (var tile in reachableTiles)
                {
                    if(!tile.IsBlocked())
                    {
                        bool canHitAnythingFromTile = CanHitAnythingFromTile(tile);
                        if(canHitAnythingFromTile)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private bool CanHitAnythingFromTile(Tile tile)
        {
            bool result = false;
            var tilesNeary = LevelBuilder.instance.GetTilesFromFourSights(tile);
            foreach (var tileNearby in tilesNeary)
            {
                if (tileNearby.IsBlocked())
                {
                    bool playerOnTile = tileNearby.type == TileType.With_Player;
                    bool destroyableObstacleOnTile = tileNearby.type == TileType.With_Obstacle 
                        && LevelBuilder.instance.obstaclesManager.IsObstacleDestroyableOnPlace(tileNearby.x, tileNearby.z);
                    if (playerOnTile || destroyableObstacleOnTile)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public void SetCurrentUnit(Unit unit)
        {
            if (_currentUnit == unit)
                return;
            _currentUnit = unit;
            _currentUnit.TryGetBotData(out _botData);
        }
    }
}