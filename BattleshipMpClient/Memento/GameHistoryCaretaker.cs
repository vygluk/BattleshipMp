using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMpClient.Memento
{
    public class GameHistoryCaretaker
    {
        private IGameStateMemento gameStateMemento;

        public void SaveGameState(Form4_GameScreen originator)
        {
            gameStateMemento = originator.SaveGameState();
        }

        public void RestoreGameState(Form4_GameScreen originator)
        {
            if (gameStateMemento != null)
            {
                originator.RestoreGameState(gameStateMemento);
            }
        }

        public void ExportGameState(string filePath)
        {
            if (gameStateMemento != null)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("----- Game Summary -----");
                stringBuilder.AppendLine($"Exported on: {DateTime.Now}");
                stringBuilder.AppendLine("Move History:");
                foreach (var move in gameStateMemento.GetMoveHistory())
                {
                    stringBuilder.AppendLine(FormatMove(move));
                }
                stringBuilder.AppendLine("-------------------------");
                stringBuilder.AppendLine("Player Board State:");
                stringBuilder.AppendLine(FormatBoardState(gameStateMemento.GetPlayerBoardState()));
                stringBuilder.AppendLine("-------------------------");
                stringBuilder.AppendLine("Opponent Board State:");
                stringBuilder.AppendLine(FormatBoardState(gameStateMemento.GetOpponentBoardState()));
                stringBuilder.AppendLine("-------------------------");

                stringBuilder.AppendLine(); // Optional: Add an empty line for better separation between exports

                // Append the game state summary to the file
                File.AppendAllText(filePath, stringBuilder.ToString());
            }
        }

        private string FormatBoardState(List<Button> boardButtons)
        {
            var boardString = new StringBuilder();
            foreach (var button in boardButtons)
            {
                // Assuming each button has a meaningful name or tag to represent its state
                boardString.AppendLine($"{button.Name} - {button.BackColor.ToString()}");
            }
            return boardString.ToString();
        }

        private string FormatMove(string move)
        {
            // Format the move string to be more descriptive (if necessary)
            // For example, you might want to translate internal move codes to human-readable descriptions
            return move;
        }
    }
}
