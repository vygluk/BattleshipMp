using System;
using System.IO;
using System.Windows.Forms;

namespace BattleshipMpClient.Facade.FacadeClasses
{
    public class AttackSender
	{
        private StreamWriter STW;

        public AttackSender(StreamWriter stw)
        {
            STW = stw;
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
    }
}
