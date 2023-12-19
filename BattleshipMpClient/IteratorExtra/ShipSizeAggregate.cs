using BattleshipMpClient.IteratorExtra;
using System.Collections.Generic;

namespace BattleshipMp.IteratorExtra
{
    public class ShipSizeAggregate : IAggregate<ShipSize>
    {
        private readonly HashSet<ShipSize> _shipSizes;

        public ShipSizeAggregate(HashSet<ShipSize> shipSizes)
        {
            _shipSizes = shipSizes;
        }

        public IIterator<ShipSize> CreateIterator()
        {
            return new ShipSizeIterator(_shipSizes);
        }
    }
}
