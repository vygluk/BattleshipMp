using BattleshipMpServer.Factory.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Visitor
{
    public class ShieldBoostVisitor : IVisitor
    {
        private Random random = new Random();
        private string upgradedShipsNames = "";

        public void VisitShip(IShip ship)
        {
            // doesn't have shields
        }

        public void VisitSpecialShip(ISpecialShip specialShip)
        {
            if (random.NextDouble() < 0.1 && specialShip.remShields > 0)
            {
                specialShip.remShields += 1;
                upgradedShipsNames += specialShip.shipName + ", ";
            }
        }

        public string GetBoostedShipNames()
        {
            return upgradedShipsNames.TrimEnd(',', ' ').Trim();
        }
    }
}
