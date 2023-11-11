using System;
using System.IO;
using System.Windows.Forms;
using BattleshipMp;
using BattleshipMpServer;

namespace BattleshipMpServer.Facade.FacadeClasses
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
            if (Server.GetInstance.IsClientConnected)
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
