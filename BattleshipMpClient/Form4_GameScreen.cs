using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BattleshipMpClient.Factory.Ship;
using BattleshipMpClient.Facade;
using BattleshipMpClient.Entity;

namespace BattleshipMpClient
{
    public partial class Form4_GameScreen : Form
    {
        private GameFacade gameFacade;
        private int currentPlayer;

        //public StreamReader STR;
        //public StreamWriter STW;
        public string recieve;
        public string TextToSend;
        List<Button> gameBoardButtons;
        List<Button> myBoardButtons;
        bool areEnabledButtons = true;
        List<(string, Color)> AllSelectedButtonList;
        bool myExit = false;
        List<Control> icebergButtons = new List<Control>();
        List<ShipButtons> icebergTiles = new List<ShipButtons>();
        List<Iceberg> icebergs = new List<Iceberg>();
        Iceberg motherIceberg = new Iceberg();

        public Form4_GameScreen(List<(string, Color)> list)
        {
            InitializeComponent(); 
            this.AllSelectedButtonList = list;
            Control.CheckForIllegalCrossThreadCalls = false;


            gameFacade = new GameFacade();

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

        private void SetObsticlesUp()
        {
            foreach (Control c in groupBox1.Controls)
            {
                if (c is Button)
                {
                    if (c.Name == "E4")
                    {
                        c.BackColor = Color.Blue;
                        icebergButtons.Add(c);
                        motherIceberg.AddTiles(c);
                    }
                    if (c.Name == "F4")
                    {
                        c.BackColor = Color.Blue;
                        icebergButtons.Add(c);
                        motherIceberg.AddTiles(c);
                    }
                }
            }

            ShipButtons tiles = new ShipButtons();
            List<string> strings = new List<string>
            {
                "E5",
                "F5"
            };

            tiles.buttonNames = strings;
            icebergTiles.Add(tiles);
            icebergs.Add(motherIceberg);
        }

        private void ExpandObsticle()
        {
            string randomTileName = GenerateRandomTile();
            Iceberg iceberg = new Iceberg();
            iceberg = (Iceberg)motherIceberg.DeepCopy();
            foreach (Control c in groupBox1.Controls)
            {
                if (c is Button && c.Name == randomTileName)
                {
                    c.BackColor = Color.Blue;
                    icebergButtons.Add(c);
                    iceberg.AddTiles(c);
                    if (CheckIfShipsTile(c))
                    {
                        var nameNumber = c.Name[1];
                        var nameToSend = $"{c.Name}{nameNumber}";
                        AttackFromEnemy(nameToSend);
                        SwitchGameButtonsEnabled();
                    }
                }
            }
        }

        private bool CheckIfShipsTile(Control c)
        {
            foreach(var tile in AllSelectedButtonList)
            {
                if(tile.Item1 == c.Name)
                {
                    return true;
                }
            }

            return false;
        }

        private string GenerateRandomTile()
        {
            Random rnd = new Random();

            int randomNumber = rnd.Next(1, 10);
            char randomLetter = (char)('A' + rnd.Next(0, 10));
            string combined =  randomLetter + randomNumber.ToString();

            return combined;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            gameBoardButtons = groupBox2.Controls.OfType<Button>().ToList();
            myBoardButtons = groupBox1.Controls.OfType<Button>().ToList();

            foreach (var item in AllSelectedButtonList)
            {
                groupBox1.Controls.Find(item.Item1, true)[0].BackColor = item.Item2;
            }

            try
            {
                gameFacade.StartGameCommunication();
                //STR = new StreamReader(Client.GetInstance.TcpClient.GetStream());
                //STW = new StreamWriter(Client.GetInstance.TcpClient.GetStream());
                //STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            timer1.Start();
            SetObsticlesUp();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int turns = 0;
            while (Client.GetInstance.IsConnected)
            {
                try
                {
                    string recieve = gameFacade.ReceiveAttack();
                    if (!string.IsNullOrEmpty(recieve))
                    {
                        AttackFromEnemy(recieve);
                        if (turns % 2 == 0)
                        {
                            ExpandObsticle();
                        }
                    }
                    //recieve = STR.ReadLine();

                    //if (recieve != "")
                    //{
                    //AttackFromEnemy(recieve);
                    //}
                    //recieve = "";
                    turns++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (Client.GetInstance.IsConnected)
            //{
            //    STW.WriteLine(TextToSend);
            //}
            //else
            //{
            //    MessageBox.Show("Message could not be sent!!");
            //}

            gameFacade.SendAttack(TextToSend);
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
            else if (recieve.Contains("miss:"))
            {
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                gameBoardButtons.FirstOrDefault(x => x.Name == result).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                richTextBox1.AppendText("Miss\n");
                return;
            }
            else if (recieve.Contains("hit:"))
            {
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                gameBoardButtons.FirstOrDefault(x => x.Name == result).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");
                richTextBox1.AppendText("Hit\n");
                return;
            }
            else if (recieve.Contains("hitShielded:"))
            {
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                gameBoardButtons.FirstOrDefault(x => x.Name == result).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\shield.png");
                richTextBox1.AppendText("Hit a shielded ship\n");
                return;
            }
            else if (recieve.Contains("youwin"))
            {
                DialogResult res = MessageBox.Show("Victory. Would you like to return to the preparation screen?", "Client - Game Result", MessageBoxButtons.YesNo);
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
                MessageBox.Show("\r\nThe opponent has left the game. You are being directed to the preparation phase.");
                this.Close();
                return;
            }

            bool isShot = false;
            bool hasShield = false;
            string shotButtonName = "";
            string shottedShip = "";
            ShipButtons deletingButton = null;

            if (Form2_PreparatoryScreen.shipList[0].shipPerButton == null)
            {
                return;
            }
            if (Form2_PreparatoryScreen.specialShipList[0].shipPerButton == null)
            {
                return;
            }
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
                            Console.WriteLine("shipList");
                            string xxx = recieve.Substring(0, recieve.Length - 1);
                            shotButtonName = xxx;
                        }
                    }
                }
            }
            foreach (var item1 in Form2_PreparatoryScreen.specialShipList)
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
                            if (item1.remShields > 0)
                            {
                                hasShield = true;
                                item1.remShields--;
                            }
                            deletingButton = item2;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("specialShipList");
                            string xxx = recieve.Substring(0, recieve.Length - 1);
                            shotButtonName = xxx;
                        }
                    }
                }
            }

            if (isShot)
            {
                if (shottedShip == "SpecialSubmarine")
                {
                    if (hasShield)
                    {
                        myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\shield.png");
                        AttackToEnemy("hitShielded:" + shotButtonName);
                        return;
                    }
                    else
                    {
                        myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");

                        AttackToEnemy("hit:" + shotButtonName);

                        deletingButton.buttonNames.Remove(shotButtonName);


                        return;
                    }
                }
                else
                {
                    myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");

                    AttackToEnemy("hit:" + shotButtonName);

                    deletingButton.buttonNames.Remove(shotButtonName);

                    //  If the hit ship is "Battleship", all buttons of "Battleship" are checked. If they're all hit, it's game over.
                    if (shottedShip == "Battleship")
                    {
                        foreach (var item in Form2_PreparatoryScreen.shipList.FirstOrDefault(x => x.shipName == "Battleship").shipPerButton)
                        {
                            if (item.buttonNames.Count > 0)
                            {
                                return;
                            }
                            AttackToEnemy("youwin");
                            DialogResult res = MessageBox.Show("You lost. Do you want to return to the preparation screen?", "Server - Game Result", MessageBoxButtons.YesNo);
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
            }
            else
            {
                if (myBoardButtons == null)
                {
                    return;
                }
                myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                AttackToEnemy("miss:" + shotButtonName);
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
                labelAttackTurn.Text = "WAIT...";
                areEnabledButtons = false;
            }
            else
            {
                foreach (var item in gameBoardButtons)
                {
                    item.Enabled = true;
                }
                labelAttackTurn.Text = "ATTACK";
                areEnabledButtons = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Client.GetInstance.IsConnected)
            {
                MessageBox.Show("Connection failed.");
                Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen(new DarkShipFactory());
                frm2.Show();
                this.Close();
            }
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!myExit)
            {
                AttackToEnemy("exitt");
            }
            Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen(new DarkShipFactory());
            frm2.Show();
        }
    }
}
