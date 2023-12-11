using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Factory.Ship
{
    public class SpecialSubmarine : ISpecialShip
    {
        public string shipName => "SpecialSubmarine";
        public int remShips { get; set; } = 1;
        public List<ShipButtons> shipPerButton { get; set; } = new List<ShipButtons>();
        public Color color { get; set; }
        public int remShields { get; set; } = 1;

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
            if (remShields > 0)
            {
                Random random = new Random();
                if (random.Next(2) == 0)
                {
                    remShields--;
                }
                else
                {
                    remShields++;
                }
            }
        }
    }
}
