using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Factory.Ship
{
    public interface IShipFactory
    {
        IShip CreateShip(ShipType type);
    }
}
