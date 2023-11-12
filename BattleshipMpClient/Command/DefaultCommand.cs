using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Command
{
    internal class DefaultCommand : ICommand
    {
        public void Execute()
        {
            // Do nothing
        }

        public void Undo()
        {
            // Do nothing
        }
    }
}
