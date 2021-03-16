using MageBattle.Core.Level;
using UnityEngine;

namespace MageBattle.Core.Units.Actions
{
    public class TrapAction : UnitsAction
    {
        private const string _obstacleId = "trap_action";

        public TrapAction(Tile target, Unit unit) : base(target, unit)
        {
            
        }

        public override void Start()
        {
            var tiles = LevelBuilder.instance.GetAllTilesNearby(target);
            var nonBlockedTiles = tiles.FindAll(tile => !tile.IsBlocked());
            foreach (var tile in nonBlockedTiles)
            {
                Debug.Log($"Tile {tile.x}:{tile.z}");
                LevelBuilder.instance.obstaclesManager.CreateObstacle(_obstacleId, tile);
            }
            OnActionCompleted();
        }

        protected override void SetId()
        {
            id = 5;
        }
        public override void Update()
        {

        }
    }
}