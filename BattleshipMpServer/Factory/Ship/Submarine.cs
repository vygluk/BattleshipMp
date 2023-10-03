using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Factory.Ship
{
    public class Submarine : IShip
    {
        public string shipName => "Submarine";
        public int remShips { get; set; } = 4;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
    }
}
