using BattleshipMpClient.IteratorExtra;
using System.Windows.Forms;

namespace BattleshipMp.IteratorExtra
{
    public class ControlAggregate : IAggregate<Control>
    {
        private readonly Control[] _controls;

        public ControlAggregate(Control[] controls)
        {
            _controls = controls;
        }

        public IIterator<Control> CreateIterator()
        {
            return new ControlIterator(_controls);
        }
    }
}