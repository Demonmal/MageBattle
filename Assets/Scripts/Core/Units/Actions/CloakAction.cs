using MageBattle.Core.Level;

namespace MageBattle.Core.Units.Actions
{
    public class CloakAction : UnitsAction
    {
        public CloakAction(Tile target, Unit unit) : base(target, unit)
        {
        }

        public override void Start()
        {
            _unit.SetInvisibility(true);
            OnActionCompleted();
        }

        protected override void SetId()
        {
            id = 4;
        }

        public override void Update()
        {

        }
    }
}