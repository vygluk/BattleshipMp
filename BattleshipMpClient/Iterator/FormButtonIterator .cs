using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMpClient.Iterator
{
    public class FormButtonIterator : IButtonIterator
    {
        private readonly List<Button> _buttons;
        private int _currentIndex = 0;

        public FormButtonIterator(List<Button> buttons)
        {
            _buttons = buttons;
        }

        public bool HasNext()
        {
            return _currentIndex < _buttons.Count;
        }

        public Button Next()
        {
            return HasNext() ? _buttons[_currentIndex++] : null;
        }
    }
}
