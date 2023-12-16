using System;
using System.Collections.Generic;

namespace BattleshipMp.IteratorExtra
{
    public class ShipSizeIterator
    {
        private readonly HashSet<ShipSize> _hashSet;
        private int _currentIndex;
        private bool hasIterated;

        public ShipSizeIterator(HashSet<ShipSize> hashSet)
        {
            _hashSet = hashSet;
            _currentIndex = 0;
            hasIterated = false;
        }

        public bool HasNext()
        {
            return !hasIterated;
        }

        public ShipSize Next()
        {
            if (HasNext())
            {
                var array = new ShipSize[_hashSet.Count];
                _hashSet.CopyTo(array);
                var nextItem = array[_currentIndex];
                _currentIndex++;

                if (_currentIndex >= _hashSet.Count)
                {
                    _currentIndex = 0;
                    hasIterated = true;
                }

                return nextItem;
            }

            return null;
        }

        public void ResetIteration()
        {
            hasIterated = false;
        }
    }
}