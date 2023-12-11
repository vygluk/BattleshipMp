using BattleshipMpServer.Factory.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Visitor
{
    public interface IVisitor
    {
        void VisitShip(IShip ship);
        void VisitSpecialShip(ISpecialShip specialShip);
    }
}
