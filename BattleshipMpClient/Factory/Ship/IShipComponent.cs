using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Factory.Ship
{
    public interface IShipComponent
    {
        string shipName { get; }
        void Add(IShipComponent component);
        void Remove(IShipComponent component);
        IShipComponent GetChild(int index);
        IEnumerable<IShipComponent> GetChildren();
        void AdjustShieldsShips();
    }
}
