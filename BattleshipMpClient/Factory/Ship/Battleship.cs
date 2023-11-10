using System.Collections.Generic;
using System.Drawing;

namespace BattleshipMpClient.Factory.Ship
{
    public class Battleship : IShip
    {
        public string shipName => "Battleship";
        public int remShips { get; set; } = 1;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
        public Color color { get; set; }
    }
}
