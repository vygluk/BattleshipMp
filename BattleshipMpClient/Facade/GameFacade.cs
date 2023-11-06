using System;
using System.IO;
using System.Windows.Forms;

namespace BattleshipMpClient.Facade
{
    public class GameFacade
    {
        private StreamReader STR;
        private StreamWriter STW;

        public GameFacade()
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

        public void SendAttack(string buttonName)
        {
            if (Client.GetInstance.IsConnected)
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
