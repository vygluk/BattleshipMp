using BattleshipMp;

namespace BattleshipMpServer.ChainOfResponsibility
{
    public class FoggyWeatherHandler : WeatherHandler
    {
        public override void HandleRequest(Form4_GameScreen gameContext)
        {
            if (!CanHandle())
            {
                _nextHandler.HandleRequest(gameContext);
            }

            gameContext.WeatherState = new Foggy();
        }
    }
}
