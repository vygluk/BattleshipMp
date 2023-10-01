using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMp
{
    public interface IShip
    {
        string shipName { get; }
        int remShips { get; set; }
        List<ShipButtons> shipPerButton { get; set; }
    }
    public class Battleship : IShip
    {
        public string shipName => "Battleship";
        public int remShips { get; set; } = 1;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
    }

    public class Cruiser : IShip
    {
        public string shipName => "Cruiser";
        public int remShips { get; set; } = 2;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
    }

    public class Destroyer : IShip
    {
        public string shipName => "Destroyer";
        public int remShips { get; set; } = 3;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
    }

    public class Submarine : IShip
    {
        public string shipName => "Submarine";
        public int remShips { get; set; } = 4;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
    }
    public class ShipButtons
    {
        public List<string> buttonNames { get; set; } = new List<string>();
    }

    public class ShipFactory
    {
        public static IShip CreateShip(string type)
        {
            switch (type)
            {
                case "Battleship":
                    return new Battleship();
                case "Cruiser":
                    return new Cruiser();
                case "Destroyer":
                    return new Destroyer();
                case "Submarine":
                    return new Submarine();
                default:
                    throw new InvalidOperationException("Invalid ship type");
            }
        }
    }
}
