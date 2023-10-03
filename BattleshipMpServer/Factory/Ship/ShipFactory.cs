using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Factory.Ship
{
    public class ShipFactory : IShipFactory
    {
        public IShip CreateShip(ShipType type)
        {
            switch (type)
            {
                case ShipType.Battleship:
                    return new Battleship();
                case ShipType.Cruiser:
                    return new Cruiser();
                case ShipType.Destroyer:
                    return new Destroyer();
                case ShipType.Submarine:
                    return new Submarine();
                default:
                    throw new InvalidOperationException("Invalid ship type");
            }
        }
    }
}
