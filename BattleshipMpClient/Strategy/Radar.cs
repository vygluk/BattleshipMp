namespace BattleshipMpClient.Strategy
{
    public class Radar
    {
        public string ScanAreaWithRandomStrategy(IRadarStrategy strategy, string buttonName)
        {
            return strategy.ScanGrid(buttonName);
        }
    }
}