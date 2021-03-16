using UnityEngine;

namespace MageBattle.Core.Units.Bots.BehaviourOperations
{
    public class CloakActionOperation : IBehaviourOperation
    {
        private const int _spellId = 4;

        public void Execute(Unit unit)
        {
            unit.actionsExecutor.Cast(_spellId, unit.currentTile);
        }
    }
}