using MageBattle.Core.Units.Bots.BehaviourOperations;

namespace MageBattle.Core.Units.Bots.BehaviourPriorities
{
    public interface IBotBehaviour
    {
        bool isAbsoluteChoice { get; }
        IBehaviourOperation operation { get; }
        void SetCurrentUnit(Unit unit);
        int GetPriority(Unit unit);
        bool IsAvailableForUnit(Unit unit);
        bool CanReachGem(Unit unit);
    }
}