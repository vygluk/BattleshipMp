using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMp.Mediator
{
    public class ServerManager
    {
        private IServerScreenMediator _mediator;

        public ServerManager(IServerScreenMediator mediator)
        {
            _mediator = mediator;
        }

        public void StartServer(string ipAddress, string port)
        {
            // Start server logic
            Server.GetInstance.ServerStart(ipAddress, port);
        }

        public void SetMediator(IServerScreenMediator mediator)
        {
            _mediator = mediator;
        }
    }

}
