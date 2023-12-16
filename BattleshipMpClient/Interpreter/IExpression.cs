namespace BattleshipMpClient.Interpreter
{
    public interface IExpression
    {
        void Interpret(InterpreterCommandContext context);
    }
}