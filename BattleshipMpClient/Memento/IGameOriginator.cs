using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Memento
{
    public interface IGameOriginator
    {
        IGameStateMemento SaveGameState();
        void RestoreGameState(IGameStateMemento memento);
    }
}
