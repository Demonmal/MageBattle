using MageBattle.Core.Level;
using MageBattle.Core.Units.Spells;
using UnityEngine;

namespace MageBattle.Core.Units.Actions
{
    public class MeteorAction : UnitsAction
    {
        public MeteorAction(Tile target, Unit unit) : base(target, unit)
        {
        }

        public override void Start()
        {
            var meteor = MeteorsPool.instance.Pop();
            Vector3 originPosition = _unit.transform.position;
            meteor.transform.position = originPosition;
            Vector3 targetPosition = new Vector3(target.transform.position.x, originPosition.y, target.transform.position.z);
            meteor.onArrived += OnMeteorArrived;
            meteor.MoveTo(targetPosition);
        }

        private void OnMeteorArrived(Meteor meteor)
        {
            meteor.onArrived -= OnMeteorArrived;
            var tiles = LevelBuilder.instance.GetAllTilesNearby(target);
            foreach (var tile in tiles)
            {
                if(tile.type == TileType.With_Obstacle)
                {
                    if (target.TryGetObstacleOnTile(out var obstacle))
                    {
                        obstacle.Destroy();
                    }
                }
                else if(tile.type == TileType.With_Player)
                {
                    if (target.TryGetUnitOnTile(out var unit))
                    {
                        unit.ReceiveDamage(Enums.DamageSource.Meteor);
                    }
                }
            }
            OnActionCompleted();
        }

        protected override void SetId()
        {
            id = 6;
        }

        public override void Update()
        {

        }
    }
}