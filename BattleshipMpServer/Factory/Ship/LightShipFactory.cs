using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Factory.Ship
{
    public class LightShipFactory : IShipFactory
    {
        public IShip CreateSubmarine()
        {
            return new Submarine() { color = Color.LightCoral };
        }

        public ISpecialShip CreateSpecialSubmarine()
        {
            return new SpecialSubmarine() { color = Color.FromArgb(220, 128, 138) };
        }

        public IShip CreateDestroyer()
        {
            return new Destroyer() { color = Color.LightSkyBlue };
        }

        public ISpecialShip CreateSpecialDestroyer()
        {
            return new SpecialDestroyer() { color = Color.SkyBlue };
        }

        public IShip CreateCruiser()
        {
            return new Cruiser() { color = Color.LightGreen };
        }

        public ISpecialShip CreateSpecialCruiser()
        {
            return new SpecialCruiser() { color = Color.Green };
        }

        public IShip CreateBattleship()
        {
            return new Battleship() { color = Color.LightGoldenrodYellow };
        }
    }
}
