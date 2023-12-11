using BattleshipMpClient.Factory.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Visitor
{
    public interface IVisitor
    {
        void VisitShip(IShip ship);
        void VisitSpecialShip(ISpecialShip specialShip);
    }
}
