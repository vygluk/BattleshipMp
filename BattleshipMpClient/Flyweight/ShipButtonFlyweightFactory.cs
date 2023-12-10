using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Flyweight
{
    public class ShipButtonFlyweightFactory
    {
        private Dictionary<Color, IShipButtonFlyweight> flyweights = new Dictionary<Color, IShipButtonFlyweight>();

        public IShipButtonFlyweight GetFlyweight(Color color)
        {
            if (!flyweights.ContainsKey(color))
            {
                flyweights[color] = new ShipButtonFlyweight(color);
            }
            return flyweights[color];
        }
    }
}
