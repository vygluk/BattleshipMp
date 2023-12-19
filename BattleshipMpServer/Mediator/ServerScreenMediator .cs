using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMp.Mediator
{
    public class ServerScreenMediator : IServerScreenMediator
    {
        private Form1_ServerScreen _form;
        private ServerManager _serverManager;
        private UIManager _uiManager;
        private FormCloseManager _formCloseManager;

        public ServerScreenMediator(Form1_ServerScreen form)
        {
            _form = form;
            _serverManager = new ServerManager(this);
            _uiManager = new UIManager(this, form);
            _formCloseManager = new FormCloseManager(this);

            _form.SetMediator(this);
            _serverManager.SetMediator(this);
            _uiManager.SetMediator(this);
            _formCloseManager.SetMediator(this);
        }

        public void Notify(object sender, string eventMessage)
        {
            switch (eventMessage)
            {
                case "StartServer":
                    string ipAddress = _form.GetIpAddress();
                    string port = _form.GetPort();
                    _serverManager.StartServer(ipAddress, port);
                    break;
                case "TimerTick":
                    _uiManager.UpdateServerStatus(Server.GetInstance.IsClientConnected, Server.GetInstance.IsListenerActive);
                    break;
                case "CloseServer":
                    _formCloseManager.CloseServer();
                    break;
            }
        }
    }

}
