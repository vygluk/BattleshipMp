using System.Collections.Generic;
using System;

namespace BattleshipMpClient.Strategy
{
    public class RadarStrategyGenerator
    {
        public IRadarStrategy GenerateRadarStrategyRandomly()
        {
            var rnd = new Random();
            var strategies = new List<IRadarStrategy>
            {
                new DiagonalRadarStrategy(),
                new HorizontalRadarStrategy(),
                new VerticalHorizontalRadarStrategy(),
                new VerticalRadarStrategy()
            };

            return strategies[rnd.Next(strategies.Count)];
        }
    }
}