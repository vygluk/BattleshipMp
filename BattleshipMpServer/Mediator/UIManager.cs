using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMp.Mediator
{
    public class UIManager
    {
        private IServerScreenMediator _mediator;
        private Form1_ServerScreen _form;

        public UIManager(IServerScreenMediator mediator, Form1_ServerScreen form)
        {
            _mediator = mediator;
            _form = form;
        }

        public void UpdateServerStatus(bool isClientConnected, bool isListenerActive)
        {
            if (isClientConnected)
            {
                _form.ServerStatusLabel.Text = "Player successfully connected.";
                _form.ContinueButton.Enabled = true;
            }
            else if (isListenerActive)
            {
                _form.ServerStatusLabel.Text = "The server is started. The player is awaited..";
            }
            else
            {
                _form.ServerStatusLabel.Text = "Waiting for the server to start...";
            }
        }

        public void SetMediator(IServerScreenMediator mediator)
        {
            _mediator = mediator;
        }
    }

}
