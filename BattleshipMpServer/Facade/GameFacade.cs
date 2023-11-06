using System;
using System.IO;
using System.Windows.Forms;
using BattleshipMp;
using BattleshipMpServer;

namespace BattleshipMpServer.Facade
{
    public class GameFacade
    {
        private StreamReader STR;
        private StreamWriter STW;

        public GameFacade()
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

        public void SendAttack(string buttonName)
        {
            if (Server.GetInstance.IsClientConnected)
            {
                STW.WriteLine(buttonName);
            }
            else
            {
                MessageBox.Show("Message could not be sent!!");
            }
        }

        public string ReceiveAttack()
        {
            return STR.ReadLine();
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
