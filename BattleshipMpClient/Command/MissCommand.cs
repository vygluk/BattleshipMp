using BattleshipMpClient.Bridge.Abstraction;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMpClient.Command
{
    public class MissCommand : ICommand
    {
        private readonly List<Button> _gameBoardButtons;
        private readonly RichTextBox _richTextBox;
        private readonly SoundPlayerBridge _missSoundPlayer;
        private readonly string _receive;

        public MissCommand(List<Button> gameBoardButtons, RichTextBox richTextBox, SoundPlayerBridge missSoundPlayer, string receive)
        {
            _gameBoardButtons = gameBoardButtons;
            _richTextBox = richTextBox;
            _missSoundPlayer = missSoundPlayer;
            _receive = receive;
        }

        public void Execute()
        {
            string result = _receive.Substring(_receive.Length - 2, 2);
            result = result + result.Substring(result.Length - 1);
            var button = _gameBoardButtons.FirstOrDefault(x => x.Name == result);
            if (button != null)
            {
                button.BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
            }
            _richTextBox.AppendText("Miss\n");
            _missSoundPlayer.Play();
        }

        public void Undo()
        {
            // If needed, implement logic to undo the MissCommand
        }
    }
}
