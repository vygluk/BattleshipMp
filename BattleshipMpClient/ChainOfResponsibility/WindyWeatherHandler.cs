namespace BattleshipMpClient.ChainOfResponsibility
{
    public class WindyWeatherHandler : WeatherHandler
    {
        public override void HandleRequest(Form4_GameScreen gameContext)
        {
            if (!CanHandle() && _nextHandler != null)
            {
                _nextHandler.HandleRequest(gameContext);
            }

            gameContext.WeatherState = new Windy();
        }
    }
}
