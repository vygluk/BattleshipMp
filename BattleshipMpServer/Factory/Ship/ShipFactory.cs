using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Factory.Ship
{
    public class ShipFactory : IShipFactory
    {
        public IShip CreateSubmarine()
        {
            return new Submarine();
        }

        public IShip CreateDestroyer()
        {
            return new Destroyer();
        }

        public IShip CreateCruiser()
        {
            return new Cruiser();
        }

        public IShip CreateBattleship()
        {
            return new Battleship();
        }
    }
}
