using MageBattle.Core.Level;

namespace MageBattle.Core.Units.Actions
{
    public class RodAction : UnitsAction
    {
        public RodAction(Tile target, Unit unit) : base(target, unit)
        {
            
        }

        public override void Start()
        {
            if(target.type == TileType.With_Obstacle)
            {
                if(target.TryGetObstacleOnTile(out var obstacle))
                {
                    obstacle.Destroy();
                }
            }
            else
            {
                if (target.TryGetUnitOnTile(out var unit))
                {
                    unit.ReceiveDamage(Enums.DamageSource.Rod);
                }
            }
            OnActionCompleted();
        }

        protected override void SetId()
        {
            id = 1;
        }

        public override void Update()
        {

        }
    }
}