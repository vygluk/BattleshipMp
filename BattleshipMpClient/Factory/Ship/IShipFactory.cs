using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Factory.Ship
{
    public interface IShipFactory
    {
        IShip CreateSubmarine();
        ISpecialShip CreateSpecialSubmarine();
        IShip CreateDestroyer();
        ISpecialShip CreateSpecialDestroyer();
        IShip CreateCruiser();
        ISpecialShip CreateSpecialCruiser();
        IShip CreateBattleship();
    }
}
