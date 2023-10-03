using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Factory.Ship
{
    public class Cruiser : IShip
    {
        public string shipName => "Cruiser";
        public int remShips { get; set; } = 2;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
    }
}
