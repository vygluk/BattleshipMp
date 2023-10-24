using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Factory.Ship
{
    public class LightShipFactory : IShipFactory
    {
        public IShip CreateSubmarine()
        {
            return new Submarine() { color = Color.LightCoral };
        }

        public ISpecialShip CreateSpecialSubmarine()
        {
            return new SpecialSubmarine() { color = Color.LightCoral };
        }
        public IShip CreateDestroyer()
        {
            return new Destroyer() { color = Color.LightSkyBlue };
        }

        public IShip CreateCruiser()
        {
            return new Cruiser() { color = Color.LightGreen };
        }

        public IShip CreateBattleship()
        {
            return new Battleship() { color = Color.LightGoldenrodYellow };
        }
    }
}
