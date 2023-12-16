namespace BattleshipMp.Interpreter
{
    public class FindShipCommandExpression : IExpression
    {
        public void Interpret(InterpreterCommandContext context)
        {
            if (context.Success)
                return;

            if (context.Input.ToLower() != "find ship")
            {
                context.Success = false;
                return;
            }

            context.Success = true;
            context.TypeOfAction = TypeOfAction.FindShip;
        }
    }
}