using BattleshipMpClient.IteratorExtra;
using System.Windows.Forms;

namespace BattleshipMp.IteratorExtra
{
    public class ControlIterator : IIterator<Control>
    {
        private readonly Control[] _array;
        private int _position = 0;

        public ControlIterator(Control[] array)
        {
            _array = array;
        }

        public bool HasNext()
        {
            if (_position < _array.Length)
                return true;

            _position = 0;
            return false;
        }

        public Control Next()
        {
            return _array[_position++];
        }
    }
}