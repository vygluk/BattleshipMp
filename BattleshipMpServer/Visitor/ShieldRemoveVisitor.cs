using BattleshipMpServer.Factory.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Visitor
{
    public class ShieldRemoveVisitor : IVisitor
    {
        public void VisitShip(IShip ship)
        {
            // doesn't have shields
        }

        public void VisitSpecialShip(ISpecialShip specialShip)
        {
            specialShip.remShields = 0;
        }
    }
}
