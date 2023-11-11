using System;
using System.IO;
using System.Windows.Forms;

namespace BattleshipMpClient.Facade.FacadeClasses
{
    public class GameCommunication
	{
        private StreamReader STR;
        private StreamWriter STW;

        public GameCommunication(StreamReader str, StreamWriter stw)
        {
            STR = str;
            STW = stw;
        }

        public void StartGameCommunication()
        {
            try
            {
                STR = new StreamReader(Client.GetInstance.TcpClient.GetStream());
                STW = new StreamWriter(Client.GetInstance.TcpClient.GetStream());
                STW.AutoFlush = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
