using MageBattle.Core.Level;

namespace MageBattle.Core.Units.Actions
{
    public class ShieldAction : UnitsAction
    {
        public ShieldAction(Tile target, Unit unit) : base(target, unit)
        {
            
        }

        public override void Start()
        {
            _unit.SetShield();
            OnActionCompleted();
        }

        protected override void SetId()
        {
            id = 3;
        }

        public override void Update()
        {

        }
    }
}