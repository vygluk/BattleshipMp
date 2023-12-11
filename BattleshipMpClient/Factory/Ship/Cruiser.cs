using BattleshipMpClient.Visitor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Factory.Ship
{
    public class Cruiser : IShip
    {
        public string shipName => "Cruiser";
        public int remShips { get; set; } = 1;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
        public Color color { get; set; }

        public void Add(IShipComponent component)
        {
            throw new InvalidOperationException("Cannot add to a leaf component");
        }

        public void Remove(IShipComponent component)
        {
            throw new InvalidOperationException("Cannot remove from a leaf component");
        }

        public IShipComponent GetChild(int index)
        {
            throw new InvalidOperationException("Leaf component has no children");
        }

        public IEnumerable<IShipComponent> GetChildren()
        {
            throw new InvalidOperationException("Leaf component has no children");
        }

        public void AdjustShieldsShips()
        {
        }

        public void Accept(IVisitor visitor)
        {
            visitor.VisitShip(this);
        }
    }
}
