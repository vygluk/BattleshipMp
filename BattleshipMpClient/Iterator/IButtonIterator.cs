using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMpClient.Iterator
{
    public interface IButtonIterator
    {
        bool HasNext();
        Button Next();
    }
}
