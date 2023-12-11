using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleshipMpServer.Visitor;

namespace BattleshipMpServer.Factory.Ship
{
    public class ShipComposite : IShipComponent
    {
        private readonly List<IShipComponent> _children = new List<IShipComponent>();
        public string shipName { get; private set; }

        public ShipComposite(string name)
        {
            shipName = name;
        }

        public void Add(IShipComponent component)
        {
            _children.Add(component);
        }

        public void Remove(IShipComponent component)
        {
            _children.Remove(component);
        }

        public IShipComponent GetChild(int index)
        {
            return _children[index];
        }

        public IEnumerable<IShipComponent> GetChildren()
        {
            return _children;
        }

        public void AdjustShieldsShips()
        {
            foreach (IShipComponent component in _children)
            {
                component.AdjustShieldsShips();
            }
        }

        public void Accept(IVisitor visitor)
        {
            foreach (var child in _children)
            {
                child.Accept(visitor);
            }
        }
    }
}
