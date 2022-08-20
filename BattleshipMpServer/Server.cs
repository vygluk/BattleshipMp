using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMp
{
    class Server
    {
        //  Soket açılıp gelecek isteklerin dinlenmeye başlandığı class.

        public static TcpListener listener;
        public static TcpClient client;

        public static void ServerStart(string ip, string port)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse(ip), int.Parse(port));
                listener.Start();
                //client = listener.AcceptTcpClient();

                listener.BeginAcceptTcpClient(AcceptClientCallback, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void AcceptClientCallback(IAsyncResult asyncResult)
        {
            client = listener.EndAcceptTcpClient(asyncResult);
        }

    }
}
