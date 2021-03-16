using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourOperations
{
    public class ShieldActionOperation : IBehaviourOperation
    {
        private const int _spellId = 3;

        public void Execute(Unit unit)
        {
            unit.actionsExecutor.Cast(_spellId, unit.currentTile);
        }
    }
}