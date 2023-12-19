using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMp.Mediator
{
    public class FormCloseManager
    {
        private IServerScreenMediator _mediator;

        public FormCloseManager(IServerScreenMediator mediator)
        {
            _mediator = mediator;
        }

        public void CloseServer()
        {
            Server.GetInstance.CloseAndDispose();

            Environment.Exit(1);
        }

        public void SetMediator(IServerScreenMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
