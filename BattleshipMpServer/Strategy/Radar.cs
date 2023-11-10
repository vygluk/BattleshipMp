namespace BattleshipMpServer.Strategy
{
    public class Radar
    {
        private IRadarStrategy _strategy;

        public Radar(IRadarStrategy strategy)
        {
            _strategy = strategy;
        }
    }
}
