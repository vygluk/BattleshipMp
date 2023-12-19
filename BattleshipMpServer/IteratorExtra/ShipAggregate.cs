using BattleshipMpClient.IteratorExtra;
using BattleshipMpServer.Factory.Ship;
using System.Collections.Generic;

namespace BattleshipMp.IteratorExtra
{
    public class ShipAggregate : IAggregate<IShip>
    {
        private readonly List<IShip> _ships;

        public ShipAggregate(List<IShip> ships)
        {
            _ships = ships;
        }

        public IIterator<IShip> CreateIterator()
        {
            return new ShipIterator(_ships);
        }
    }
}