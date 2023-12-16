using BattleshipMpServer.Factory.Ship;
using System.Collections.Generic;

namespace BattleshipMp.IteratorExtra
{
    public class ShipListIterator
    {
        private List<IShip> _list;
        private int _position = 0;

        public ShipListIterator(List<IShip> list)
        {
            _list = list;
        }

        public bool HasNext()
        {
            if (_position < _list.Count )
                return true;

            _position = 0;
            return false;
        }

        public IShip Next()
        {
            return _list[_position++];
        }
    }
}