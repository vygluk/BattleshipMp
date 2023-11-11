using System;
using System.IO;
using System.Windows.Forms;
using BattleshipMp;
using BattleshipMpServer;
using BattleshipMpServer.Facade.FacadeClasses;

namespace BattleshipMpServer.Facade
{
    public class GameFacade
    {
        private StreamReader STR;
        private StreamWriter STW;

        private AttackSender attackSender;
        private AttackReceiver attackReceiver;
        private GameCommunication gameCommunication;

        public GameFacade()
        {
            try
            {
                STR = new StreamReader(Server.GetInstance.Client.GetStream());
                STW = new StreamWriter(Server.GetInstance.Client.GetStream());
                STW.AutoFlush = true;

                attackSender = new AttackSender(STW);
                attackReceiver = new AttackReceiver(STR);
                gameCommunication = new GameCommunication(STR, STW);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public AttackSender GetAttackSender()
        {
            return attackSender;
        }

        public AttackReceiver GetAttackReceiver()
        {
            return attackReceiver;
        }

        public GameCommunication GetGameCommunication()
        {
            return gameCommunication;
        }
    }
}
