using System.Text.RegularExpressions;

namespace BattleshipMpClient.Interpreter
{
    public class AttackCommandExpression : IExpression
    {
        public void Interpret(InterpreterCommandContext context)
        {
            if (context.Success)
                return;

            var writtenText = context.Input.ToLower();
            var match = Regex.Match(writtenText, @"^attack ([a-j])(10|[1-9])$");

            if (!match.Success)
            {
                context.Success = false;
                return;
            }

            char row = match.Groups[1].Value.ToUpper()[0];
            int column = int.Parse(match.Groups[2].Value) - 1;

            context.Output = $"{row}{column}{column}";
            context.Success = true;
        }
    }
}