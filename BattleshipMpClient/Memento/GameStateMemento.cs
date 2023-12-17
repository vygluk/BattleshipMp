using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMpClient.Memento
{
    public class GameStateMemento : IGameStateMemento
    {
        private List<Button> playerBoardState;
        private List<Button> opponentBoardState;
        private List<string> moveHistory;

        public GameStateMemento(List<Button> playerBoard, List<Button> opponentBoard, List<string> moves)
        {
            playerBoardState = playerBoard;
            opponentBoardState = opponentBoard;
            moveHistory = moves;
        }

        public List<Button> GetPlayerBoardState() => playerBoardState;
        public List<Button> GetOpponentBoardState() => opponentBoardState;
        public List<string> GetMoveHistory() => moveHistory;
    }
}
