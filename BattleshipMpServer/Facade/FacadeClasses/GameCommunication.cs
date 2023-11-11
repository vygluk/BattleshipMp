using System;
using System.IO;
using System.Windows.Forms;
using BattleshipMp;
using BattleshipMpServer;

namespace BattleshipMpServer.Facade.FacadeClasses
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
                STR = new StreamReader(Server.GetInstance.Client.GetStream());
                STW = new StreamWriter(Server.GetInstance.Client.GetStream());
                STW.AutoFlush = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
