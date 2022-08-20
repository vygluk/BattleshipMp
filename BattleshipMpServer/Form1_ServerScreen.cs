using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace BattleshipMp
{
    public partial class Form1_ServerScreen : Form
    {

        public Form1_ServerScreen()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;

            textBoxIpAddress.Text = FillIpTextBox();
        }


        //  IP alanına bilgisayarın v4 ipsini getir. (cmd'deki ipconfig)
        string FillIpTextBox()
        {
            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());

            string ipadr = "";

            foreach (var address in localIp)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipadr = address.ToString();
                }
            }
            return ipadr;
        }

        //  IP alanından başka bir alana geçildiğinde içeriği bilgisayar'ın ipsi ya da localhost olup olmadığı kontrolünü yap.
        private void textBoxIpAddress_Leave(object sender, EventArgs e)
        {
            if (textBoxIpAddress.Text != "127.0.0.1" && textBoxIpAddress.Text != FillIpTextBox())
            {
                textBoxIpAddress.Text = FillIpTextBox();
            }
        }

        //  Server classındaki ServerStart metodunu çalıştır.
        private void buttonServerStart_Click(object sender, EventArgs e)
        {
            Server.ServerStart(textBoxIpAddress.Text, textBoxPort.Text);
        }

        //  1 sn'de bir Client'in bağlanıp bağlanmadığını kontrol et. Buna göre "Devam" butonunu aktifleştir.
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Server.client != null && Server.client.Connected)
            {
                labelServerState.Text = "Oyuncu başarıyla bağlandı.";
                buttonGoToBoard.Enabled = true;
            }
            else if (Server.listener != null)
            {
                labelServerState.Text = "Server başlatıldı. Oyuncu bekleniyor..";
            }
            else
            {
                labelServerState.Text = "Server'ın başlatılması bekleniyor..";
            }
        }

        //  Hazırlık aşamasını içeren Form2'ye geç.
        private void buttonGoToBoard_Click(object sender, EventArgs e)
        {

            timer1.Stop();

            Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen();
            frm2.Show();
            this.Visible = false;
        }

        

        private void Form1_ServerScreen_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void Form1_ServerScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Server.client.Close();
            Server.client.Dispose();
            Server.client = null;

            Server.listener.Stop();
            Server.listener = null;

            Environment.Exit(1);
        }
    }
}
