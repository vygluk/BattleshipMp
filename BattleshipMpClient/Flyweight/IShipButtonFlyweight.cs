using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Flyweight
{
    public interface IShipButtonFlyweight
    {
        void DisplayShipButton(ShipButtonContext context);
    }
}
