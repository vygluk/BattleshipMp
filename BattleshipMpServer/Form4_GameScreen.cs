using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleshipMpServer.Factory.Ship;
using BattleshipMpServer.Facade;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace BattleshipMp
{
    public partial class Form4_GameScreen : Form
    {
        private GameFacade gameFacade;

        //public StreamReader STR;
        //public StreamWriter STW;
        public string recieve;
        public string TextToSend;
        List<Button> gameBoardButtons;
        List<Button> myBoardButtons;
        bool areEnabledButtons = true;
        List<(string, Color)> AllSelectedButtonList;
        bool myExit = false;

        //  While creating the "game screen" object, get the list of selected buttons from Form2 and change their color with the help of constructor.
        public Form4_GameScreen(List<(string, Color)> list)
        {
            InitializeComponent();
            this.AllSelectedButtonList = list;
            Control.CheckForIllegalCrossThreadCalls = false;

            gameFacade = new GameFacade();
        }

        //  Replace the mouse pointer with a red target image while making moves. return to normal pointer when the button is over.
        private void button_mousehover(object sender, EventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Default;
        }
        private void button_mouseleave(object sender, EventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Default;
        }


        private void Form4_GameScreen_Load(object sender, EventArgs e)
        {
            // 1 // Save all the buttons on the game board and the selected ships to the list.
            gameBoardButtons = groupBox2.Controls.OfType<Button>().ToList();
            myBoardButtons = groupBox1.Controls.OfType<Button>().ToList();

            // 2 // Change the color of the ships in the "My Ships" section according to the button list from Form2.
            foreach (var item in AllSelectedButtonList)
            {
                groupBox1.Controls.Find(item.Item1, true)[0].BackColor = item.Item2;
            }

            // 3 // Create Streams that will provide data exchange between Server and Client and assign them to global variables.
            // Since the "backgorundWorker1" object will always listen for incoming data, it will be running in the background all the time.
            try
            {
                gameFacade.StartGameCommunication();
                //STR = new StreamReader(Server.GetInstance.Client.GetStream());
                //STW = new StreamWriter(Server.GetInstance.Client.GetStream());
                //STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            // 4 // When first starting the game, generate a random number of 0 or 1 to determine whose turn it is. Send this to the other side at the start of the game.
            // Activate or deactivate the buttons on the game board after the order of moves has been determined.
            Random random = new Random();
            int rnd = random.Next(0, 2);
            if (rnd == 0)
            {
                areEnabledButtons = true;
                SwitchGameButtonsEnabled();
            }
            else
            {
                areEnabledButtons = false;
                SwitchGameButtonsEnabled();
            }
            AttackToEnemy(rnd.ToString());

            timer1.Start();
        }

        // 5 // Read the incoming information continuously if the connection is provided. If the incoming information is not empty, execute the "AttackFromEnemy()" method.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Server.GetInstance.IsClientConnected)
            {
                try
                {
                    string recieve = gameFacade.ReceiveAttack();
                    if (!string.IsNullOrEmpty(recieve))
                    {
                        AttackFromEnemy(recieve);
                    }
                    //recieve = STR.ReadLine();

                    //if (recieve != "")
                    //{
                    //    AttackFromEnemy(recieve);
                    //}
                    //recieve = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        // 6 // This is the method where the important operations are done. Necessary explanations are in the method. When a move is made, this method is reached with more than 1 iteration.
        // It's also mentioned in the description.
        private void AttackFromEnemy(string recieve)
        {
            //  Read the data that determines who is next in step four. If the data is 0, Server will start; If the data is 1, Client will start.
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

            //  Information about whether the attacking party hit the target or not is written to the RichTextBox object in the interface.
            //  If it is a miss, the background of the button is tiled with the "O" image, otherwise the "X" image is used.
            //  This process takes place in the 2nd iteration.
            else if (recieve.Contains("miss:"))
            {
                //  String operations edit the received button names because the player's ships and the board on which the game is played are separate.
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

            // Even if all boxes are hit, the game will end only when the ship "Battleship" is hit; If the incoming data is "youwin",
            // the party receiving this message wins the game. He is asked if he wants to go to the preparation stage for the new game. Action is taken according to the situation.
            // If the game is over, this data is read in the 3rd iteration.
            else if (recieve.Contains("youwin"))
            {
                DialogResult res = MessageBox.Show("Victory. Would you like to return to the preparation screen?", "Server - Game Result", MessageBoxButtons.YesNo);
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

            //  Variables held for the outcome of the hit.
            bool isShot = false;
            bool hasShield = false;
            string shotButtonName = "";
            string shottedShip = "";
            ShipButtons deletingButton = null;

            //  If the above if conditions are not met, the method considers it as a move.
            //  The counter move comes with the button name. Then it is searched among the buttons of the ship according to the structure in the "Ship" model.
            //  Depending on whether the attack hit the target or not, values ​​are assigned to the above variables.
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
                            string xxx = recieve.Substring(0, recieve.Length - 1);
                            shotButtonName = xxx;
                        }
                    }
                }
            }


            //  If the target has been hit, the button on the target board is marked. Then it sends hit data across. Yine aynı AttackFromEnemy() metodu
            //  Again, the same AttackFromEnemy() method operates according to the incoming hit or miss data. + if(recieve.Contain(miss)) statement.
            //  The button of the hit ship is deleted from the list.
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
                } else
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

            //  If the target is not hit, only the background of the relevant button is changed and in the 2nd iteration, data is sent to read this part again.
            else
            {
                myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                AttackToEnemy("miss:" + shotButtonName);
                return;
            }
        }

        //  The object to which the data will be sent. It is executed only when a attack is made. Otherwise it waits.
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (Server.GetInstance.IsClientConnected)
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

        //  Click events of all buttons on which attacks will be made are associated with this method. When clicked, it sends the name of the button to enemy.
        private void button_click(object sender, EventArgs e)
        {
            AttackToEnemy(((Button)sender).Name);
        }

        //  Makes adjustments to the button name while sending data. A11 shaped button sends its name as A1.
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

        //  The method to activate or deactivate the buttons on the attack board according to the attack order.
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
            if (Server.GetInstance.IsListenerActive && !Server.GetInstance.IsClientConnected)
            {
                MessageBox.Show("Client connection failed.");
            }
        }

        //  When closing the form. Is it because the game is over? Or is it closing while the game is in progress,
        //  check this. If it is in progress, send information that the player has left the game.
        private void Form4_GameScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!myExit)
            {
                AttackToEnemy("exitt");
            }
            Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen(new LightShipFactory());
            frm2.Show();
        }
    }
}
