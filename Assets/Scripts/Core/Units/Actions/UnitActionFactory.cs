using MageBattle.Core.Level;

namespace MageBattle.Core.Units.Actions
{
    public static class UnitActionFactory
    {
        public static UnitsAction CreateAction(int actionId, Unit unit, Tile tile)
        {
            UnitsAction action;

            switch (actionId)
            {
                case 1:
                    action = new RodAction(tile, unit);
                    break;
                case 2:
                    action = new BlinkAction(tile, unit);
                    break;
                case 3:
                    action = new ShieldAction(tile, unit);
                    break;
                case 4:
                    action = new CloakAction(tile, unit);
                    break;
                case 5:
                    action = new TrapAction(tile, unit);
                    break;
                case 6:
                    action = new MeteorAction(tile, unit);
                    break;
                default:
                    action = new MoveAction(tile, unit);
                    break;
            }

            return action;
        }
    }
}