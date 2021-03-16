using MageBattle.Core.Level;
using UnityEngine;

namespace MageBattle.Core.Units.Actions
{
    public class BlinkAction : UnitsAction
    {
        private const float _offsetY = 0.7f;

        public BlinkAction(Tile target, Unit unit) : base(target, unit)
        {

        }

        public override void Start()
        {
            Vector3 position = new Vector3(target.transform.position.x, _offsetY, target.transform.position.z);
            _unit.transform.position = position;
            _unit.SetCurrentTile(target);
            OnActionCompleted();
        }

        protected override void SetId()
        {
            id = 2;
        }

        public override void Update()
        {
            
        }
    }
}