using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Factory.Ship
{
    public class Destroyer : IShip
    {
        public string shipName => "Destroyer";
        public int remShips { get; set; } = 3;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
    }
}
