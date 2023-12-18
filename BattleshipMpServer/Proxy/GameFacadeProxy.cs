using SharedFile.Facade.FacadeClasses;
using SharedFile.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMp.Proxy
{
    public class GameFacadeProxy
    {
        private GameFacade realGameFacade;
        private readonly ITcpStreamProvider tcpStreamProvider;

        public GameFacadeProxy(ITcpStreamProvider tcpStreamProvider)
        {
            this.tcpStreamProvider = tcpStreamProvider;
        }

        private GameFacade RealGameFacade
        {
            get
            {
                if (realGameFacade == null)
                {
                    realGameFacade = new GameFacade(tcpStreamProvider);
                }
                return realGameFacade;
            }
        }

        public AttackSender GetAttackSender()
        {
            return RealGameFacade.GetAttackSender();
        }

        public AttackReceiver GetAttackReceiver()
        {
            return RealGameFacade.GetAttackReceiver();
        }

        public GameCommunication GetGameCommunication()
        {
            return RealGameFacade.GetGameCommunication();
        }
    }
}
