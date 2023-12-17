using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMp.Memento
{
    public interface IGameOriginator
    {
        IGameStateMemento SaveGameState();
        void RestoreGameState(IGameStateMemento memento);
    }
}
