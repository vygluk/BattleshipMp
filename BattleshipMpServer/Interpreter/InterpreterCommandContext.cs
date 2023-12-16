namespace BattleshipMp.Interpreter
{
    public class InterpreterCommandContext
    {
        public string Input { get; set; }

        public string Output { get; set; }

        public TypeOfAction TypeOfAction { get; set; }

        public bool Success { get; set; }

        public void ActOnInterpretedOutput(Form4_GameScreen gameScreen)
        {
            if (!Success)
                return;

            if (TypeOfAction == TypeOfAction.Attack)
            {
                gameScreen.AttackToEnemy(Output);
            }
            else if (TypeOfAction == TypeOfAction.FindShip)
            {
                if (gameScreen.playerItem.remItems > 0)
                {
                    gameScreen.AttackToEnemy("[EnemyItem]");
                    gameScreen.playerItem.remItems--;
                }
            }

            ResetProperties();
        }

        private void ResetProperties()
        {
            Input = default;
            Output = default;
            TypeOfAction = default;
            Success = default;
        }
    }
}