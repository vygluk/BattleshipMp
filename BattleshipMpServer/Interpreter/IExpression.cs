﻿namespace BattleshipMp.Interpreter
{
    public interface IExpression
    {
        void Interpret(InterpreterCommandContext context);
    }
}