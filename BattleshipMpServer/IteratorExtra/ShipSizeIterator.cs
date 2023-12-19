using BattleshipMpClient.IteratorExtra;
using System.Collections.Generic;

namespace BattleshipMp.IteratorExtra
{
    public class ShipSizeIterator : IIterator<ShipSize>
    {
        private readonly HashSet<ShipSize> _hashSet;
        private int _currentIndex;

        public ShipSizeIterator(HashSet<ShipSize> hashSet)
        {
            _hashSet = hashSet;
            _currentIndex = 0;
        }

        public bool HasNext()
        {
            if (_currentIndex < _hashSet.Count)
                return true;

            _currentIndex = 0;

            return false;
        }

        public ShipSize Next()
        {
            if (HasNext())
            {
                var array = new ShipSize[_hashSet.Count];
                _hashSet.CopyTo(array);
                var nextItem = array[_currentIndex];
                _currentIndex++;

                return nextItem;
            }

            return null;
        }
    }
}