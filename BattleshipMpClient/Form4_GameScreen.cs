using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMpClient
{
    public partial class Form4_GameScreen : Form
    {
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;
        List<Button> gameBoardButtons;
        List<Button> myBoardButtons;
        bool areEnabledButtons = true;
        List<string> AllSelectedButtonList;
        bool myExit = false;


        public Form4_GameScreen(List<string> list)
        {
            InitializeComponent();
            this.AllSelectedButtonList = list;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button_mousehover(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(new Bitmap(Application.StartupPath + @"\Images\target.png"), 20, 20);
            ((Button)sender).Cursor = new Cursor(bm.GetHicon());
        }
        private void button_mouseleave(object sender, EventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Default;
        }


        private void Form4_Load(object sender, EventArgs e)
        {
            gameBoardButtons = groupBox2.Controls.OfType<Button>().ToList();
            myBoardButtons = groupBox1.Controls.OfType<Button>().ToList();

            foreach (var item in AllSelectedButtonList)
            {
                groupBox1.Controls.Find(item, true)[0].BackColor = Color.DarkGray;
            }

            try
            {
                STR = new StreamReader(Client.client.GetStream());
                STW = new StreamWriter(Client.client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            timer1.Start();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Client.client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();

                    if (recieve != "")
                    {
                        AttackFromEnemy(recieve);
                    }
                    recieve = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Client.client.Connected)
            {
                STW.WriteLine(TextToSend);
            }
            else
            {
                MessageBox.Show("Mesaj Gönderilemedi!!");
            }

            backgroundWorker2.CancelAsync();
        }

        private void button_click(object sender, EventArgs e)
        {
            AttackToEnemy(((Button)sender).Name);
        }

        private void AttackFromEnemy(string recieve)
        {
            if (recieve == "0")
            {
                areEnabledButtons = true;
                SwitchGameButtonsEnabled();
                return;
            }
            else if (recieve == "1")
            {
                areEnabledButtons = false;
                SwitchGameButtonsEnabled();
                return;
            }
            else if (recieve.Contains("karavana:"))
            {
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                gameBoardButtons.FirstOrDefault(x => x.Name == result).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                richTextBox1.AppendText("Karavana\n");
                return;
            }
            else if (recieve.Contains("isabet:"))
            {
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                gameBoardButtons.FirstOrDefault(x => x.Name == result).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");
                richTextBox1.AppendText("İsabet\n");
                return;
            }
            else if (recieve.Contains("youwin"))
            {
                DialogResult res = MessageBox.Show("Kazandın. Hazırlık ekranına dönmek ister misin?", "Server - Oyun Sonucu", MessageBoxButtons.YesNo);
                {
                    if (res == DialogResult.Yes)
                    {
                        myExit = true;
                        this.Close();
                    }
                    else
                        Environment.Exit(1);
                }
                return;
            }

            else if (recieve.Contains("exit"))
            {
                MessageBox.Show("Rakip oyunu terketti. Hazırlık aşamasına yönlendirileceksiniz.");
                this.Close();
            }

            bool isShot = false;
            string shotButtonName = "";
            string shottedShip = "";
            ShipButtons deletingButton = null;

            foreach (var item1 in Form2_PreparatoryScreen.shipList)
            {
                foreach (var item2 in item1.shipPerButton)
                {
                    foreach (string item3 in item2.buttonNames)
                    {
                        if (item3 == recieve.Substring(0, recieve.Length - 1))
                        {
                            isShot = true;
                            shotButtonName = item3;
                            shottedShip = item1.shipName;
                            deletingButton = item2;
                            break;
                        }
                        else
                        {
                            string xxx = recieve.Substring(0, recieve.Length - 1);
                            shotButtonName = xxx;
                        }
                    }
                }
            }
            
            if (isShot)
            {
                myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");

                AttackToEnemy("isabet:" + shotButtonName);

                deletingButton.buttonNames.Remove(shotButtonName);

                if (shottedShip == "Amiral")
                {
                    foreach (var item in Form2_PreparatoryScreen.shipList.FirstOrDefault(x => x.shipName == "Amiral").shipPerButton)
                    {
                        if (item.buttonNames.Count > 0)
                        {
                            return;
                        }

                        AttackToEnemy("youwin");
                        DialogResult res = MessageBox.Show("Kaybettin. Hazırlık ekranına dönmek ister misin?", "Server - Oyun Sonucu", MessageBoxButtons.YesNo);
                        {
                            if (res == DialogResult.Yes)
                            {
                                myExit = true;
                                this.Close();
                            }
                            else
                                Environment.Exit(1);
                        }
                        return;
                    }
                }
                return;
            }
            else
            {
                myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                AttackToEnemy("karavana:" + shotButtonName);
                return;
            }

        }

        private void AttackToEnemy(string buttonName)
        {
            if (buttonName == null)
            {
                return;
            }

            else if (buttonName.Length == 3)
            {
                TextToSend = buttonName.Substring(0, 2);
            }

            TextToSend = buttonName;
            backgroundWorker2.RunWorkerAsync();

            SwitchGameButtonsEnabled();
        }

        private void SwitchGameButtonsEnabled()
        {
            if (areEnabledButtons == true)
            {
                foreach (var item in gameBoardButtons)
                {
                    item.Enabled = false;
                }
                labelAttackTurn.Text = "BEKLE...";
                areEnabledButtons = false;
            }
            else
            {
                foreach (var item in gameBoardButtons)
                {
                    item.Enabled = true;
                }
                labelAttackTurn.Text = "HAMLE YAP";
                areEnabledButtons = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Client.client.Connected == false)
            {
                MessageBox.Show("Bağlantı koptu.");
                Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen();
                frm2.Show();
                this.Close();
            }
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!myExit)
            {
                AttackToEnemy("exit");
            }
            Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen();
            frm2.Show();
        }
    }
}
