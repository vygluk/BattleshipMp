using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleshipMpClient.Factory.Ship;

namespace BattleshipMpClient
{
    public partial class Form1_ClientScreen : Form
    {
        public Form1_ClientScreen()
        {
            InitializeComponent();
        }

        private void buttonConnectToServer_Click(object sender, EventArgs e)
        {
            Client.ConnectToServer(textBoxIpAddress.Text, textBoxPort.Text);

            if (Client.client != null)
            {
                buttonGoToBoard.Enabled = true;
                labelServerState.Text = "Connection successful. You can continue.";
            }
            else
                labelServerState.Text = "You must connect to the server.";
        }

        //  Go to ship theme selection form, Form12
        private void buttonGoToBoard_Click(object sender, EventArgs e)
        {


            Form12_ShipThemeSelection frmThemeSelection = new Form12_ShipThemeSelection();
            frmThemeSelection.Show();

            this.Visible = false;
        }

        private void Form1_ClientScreen_Load(object sender, EventArgs e)
        {
            labelServerState.Text = "You must connect to the server.";
        }

        private void Form1_ClientScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client.client.Close();
            Client.client.Dispose();
            Client.client = null;
            Environment.Exit(1);
        }
    }
}
