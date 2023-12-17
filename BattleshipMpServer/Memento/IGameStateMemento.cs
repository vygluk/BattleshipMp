using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMp.Memento
{
    public interface IGameStateMemento
    {
        List<Button> GetPlayerBoardState();
        List<Button> GetOpponentBoardState();
        List<string> GetMoveHistory();
    }
}
