using System;

namespace BattleshipMpClient.ChainOfResponsibility
{
    public abstract class WeatherHandler
    {
        private Random _rnd = new Random();

        protected WeatherHandler _nextHandler;
        public void SetNext(WeatherHandler handler)
        {
            _nextHandler = handler;
        }

        protected bool CanHandle()
        {
            return _rnd.NextDouble() >= 0.5;
        }

        public abstract void HandleRequest(Form4_GameScreen gameContext);
    }
}