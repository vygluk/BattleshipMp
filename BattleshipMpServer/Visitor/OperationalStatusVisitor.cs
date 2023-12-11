using BattleshipMpServer.Factory.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Visitor
{
    public class OperationalStatusVisitor : IVisitor
    {
        private string destroyedShipsNames = "";

        public void VisitShip(IShip ship)
        {
            if (!ship.shipPerButton[0].buttonNames.Any())
            {
                destroyedShipsNames += ship.shipName + ", ";
            }
        }

        public void VisitSpecialShip(ISpecialShip specialShip)
        {
            if (!specialShip.shipPerButton[0].buttonNames.Any())
            {
                destroyedShipsNames += specialShip.shipName + ", ";
            }
        }

        public string GetNonOperationalShipTypes()
        {
            return destroyedShipsNames.TrimEnd(',', ' ').Trim();
        }
    }
}
