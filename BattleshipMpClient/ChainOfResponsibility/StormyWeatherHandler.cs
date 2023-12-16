namespace BattleshipMpClient.ChainOfResponsibility
{
    internal class StormyWeatherHandler : WeatherHandler
    {
        public override void HandleRequest(Form4_GameScreen gameContext)
        {
            if (!CanHandle())
            {
                gameContext.WeatherState = new Windless();
                return;
            }

            gameContext.WeatherState = new Stormy();
        }
    }
}