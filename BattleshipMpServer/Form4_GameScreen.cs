using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BattleshipMpServer.Factory.Ship;
using BattleshipMpServer.Facade;
using BattleshipMpServer.Entity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using BattleshipMpServer.Strategy;
using BattleshipMpServer.Observer;
using BattleshipMp.Builder;
using BattleshipMp.Adapter;
using BattleshipMpServer.Bridge.Abstraction;
using BattleshipMpServer.Bridge.Concrete;

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
        List<Control> icebergButtons = new List<Control>();
        List<ShipButtons> icebergTiles = new List<ShipButtons>();
        List<Iceberg> icebergs = new List<Iceberg>();
        Iceberg motherIceberg = new Iceberg();
        bool myExit = false;
        bool enemyHasUsedRadarUse = false;
        bool hasRadarUse = true;
        bool enemyReceivedExtraRound = false;
        bool weHaveReceivedExtraRound = false;
        private readonly RadarStrategyGenerator _radarStrategyGenerator;
        private readonly ExtraRoundSubscriberMap _extraRoundSubscriberMap;
        private readonly ExtraRoundPublisher _extraRoundPublisher;
        private IcebergShipInteractionAdapter icebergShipInteractionAdapter = new IcebergShipInteractionAdapter();
        private const int PERCENTAGE_MAX = 100;
        private HashSet<string> clickedButtons = new HashSet<string>();
        bool isIceberg = false;
        bool skipIcebergChange = false;
        int turns = 0;

        //  While creating the "game screen" object, get the list of selected buttons from Form2 and change their color with the help of constructor.
        public Form4_GameScreen(List<(string, Color)> list)
        {
            InitializeComponent();
            this.AllSelectedButtonList = list;
            Control.CheckForIllegalCrossThreadCalls = false;

            _radarStrategyGenerator = new RadarStrategyGenerator();
            _extraRoundSubscriberMap = new ExtraRoundSubscriberMap();
            _extraRoundPublisher = new ExtraRoundPublisher();

            foreach (var subscriber in _extraRoundSubscriberMap.GetSubscribers())
            {
                _extraRoundPublisher.Subscribe(subscriber);
            }

            gameFacade = new GameFacade();
        }

        //  Replace the mouse pointer with a red target image while making moves. return to normal pointer when the button is over.
        private void button_mousehover(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                Bitmap bm = new Bitmap(new Bitmap(Application.StartupPath + @"\Images\target.png"), 20, 20);
                button.Cursor = new Cursor(bm.GetHicon());
            }
        }

        private void button_mouseleave(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.Cursor = Cursors.Default;
            }
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
                gameFacade.GetGameCommunication().StartGameCommunication();
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
            SetObsticlesUp();
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
                    icebergShipInteractionAdapter.ProcessIcebergShipCollision(iceberg, AllSelectedButtonList, this, out isIceberg);

                }
            }
        }

        private bool CheckIfShipsTile(Control c)
        {
            foreach (var tile in AllSelectedButtonList)
            {
                if (tile.Item1 == c.Name)
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
            string combined = randomLetter + randomNumber.ToString();

            return combined;
        }

        // 5 // Read the incoming information continuously if the connection is provided. If the incoming information is not empty, execute the "AttackFromEnemy()" method.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int turns = 0;
            while (Server.GetInstance.IsClientConnected)
            {
                try
                {
                    string recieve = gameFacade.GetAttackReceiver().ReceiveAttack();
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
                    turns++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        // 6 // This is the method where the important operations are done. Necessary explanations are in the method. When a move is made, this method is reached with more than 1 iteration.
        // It's also mentioned in the description.
        public void AttackFromEnemy(string recieve)
        {
            ISoundImplementation hitSoundImplementation = new HitSound();
            ISoundImplementation missSoundImplementation = new MissSound();
            SoundPlayerBridge hitSoundPlayer = new HitSoundPlayer(hitSoundImplementation);
            SoundPlayerBridge missSoundPlayer = new MissSoundPlayer(missSoundImplementation);
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
                var button = gameBoardButtons.FirstOrDefault(x => x.Name == result);
                if (button != null)
                {
                    button.BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                }
                richTextBox1.AppendText("Miss\n");
                missSoundPlayer.Play();
                return;
            }
            else if (recieve.Contains("hit:"))
            {
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                var button = gameBoardButtons.FirstOrDefault(x => x.Name == result);
                if (button != null)
                {
                    button.BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");
                    button.Enabled = false;
                }
                richTextBox1.AppendText("Hit\n");
                hitSoundPlayer.Play();
                return;
            }
            else if (recieve.Contains("hitShielded:"))
            {
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                gameBoardButtons.FirstOrDefault(x => x.Name == result).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\shield.png");
                richTextBox1.AppendText("Hit a shielded ship\n");
                hitSoundPlayer.Play();
                return;
            }

            // Even if all boxes are hit, the game will end only when the ship "Battleship" is hit; If the incoming data is "youwin",
            // the party receiving this message wins the game. He is asked if he wants to go to the preparation stage for the new game. Action is taken according to the situation.
            // If the game is over, this data is read in the 3rd iteration.
            else if (recieve.Contains("youwin"))
            {
                DialogResult res = MessageBox.Show("Victory.", "Server - Game Result", MessageBoxButtons.OK);
                {
                    if (res == DialogResult.Yes)
                    {
                        Environment.Exit(1);
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
            else if (recieve.Contains("[Radar]"))
            {
                richTextBox1.AppendText($"{recieve}\n");
                hasRadarUse = false;
                return;
            }
            else if (recieve.Contains("Extra round"))
            {
                weHaveReceivedExtraRound = true;
                skipIcebergChange = true;
                SwitchGameButtonsEnabled();
                richTextBox1.AppendText($"[Lucky] The block you selected gave you extra round!\n");
                return;
            }

            if (!enemyHasUsedRadarUse)
            {
                var radar = new Radar(_radarStrategyGenerator);

                var buttonToShoot = recieve.Substring(0, recieve.Length - 1);
                var message = radar.ScanAreaWithRandomStrategy(buttonToShoot);

                AttackToEnemy($"[Radar] {message}");

                enemyHasUsedRadarUse = true;
                return;
            }

            var extraSubscriberToGet = recieve.Substring(0, recieve.Length - 1);
            var rnd = new Random();
            var extraSubscriberOnClickedButton = _extraRoundSubscriberMap.GetExtraRoundSubscriber(extraSubscriberToGet);
            enemyReceivedExtraRound = extraSubscriberOnClickedButton.GetExtraRoundChancePercentages() > rnd.Next(PERCENTAGE_MAX + 1);
            if (enemyReceivedExtraRound)
            {
                AttackToEnemy("Extra round");
                SwitchGameButtonsEnabled();
            }

            if (!isIceberg)
            {
                if (!enemyReceivedExtraRound && !weHaveReceivedExtraRound)
                {
                    if (turns % 2 == 0)
                    {
                        ExpandObsticle();
                        turns = 0;
                    }
                    else
                    {
                        turns++;
                    }
                }
            }

            //  Variables held for the outcome of the hit.
            bool hasShield = false;
            bool isShot = false;
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

            _extraRoundPublisher.NotifySubscribers();

            if (isShot)
            {
                if (shottedShip == "SpecialSubmarine" || shottedShip == "SpecialCruiser" || shottedShip == "SpecialDestroyer" || shottedShip == "Battleship")
                {
                    if (hasShield)
                    {
                        myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\shield.png");
                        AttackToEnemy("hitShielded:" + shotButtonName);
                        return;
                    }
                    else
                    {
                        _extraRoundPublisher.Unsubscribe(_extraRoundSubscriberMap.GetExtraRoundSubscriber(extraSubscriberToGet));
                        myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");

                        AttackToEnemy("hit:" + shotButtonName);

                        deletingButton.buttonNames.Remove(shotButtonName);

                        //  If the hit ship is "Battleship", all buttons of "Battleship" are checked. If they're all hit, it's game over.
                        if (shottedShip == "Battleship")
                        {
                            foreach (var item in Form2_PreparatoryScreen.specialShipList.FirstOrDefault(x => x.shipName == "Battleship").shipPerButton)
                            {
                                if (item.buttonNames.Count > 0)
                                {
                                    return;
                                }
                                AttackToEnemy("youwin");
                                DialogResult res = MessageBox.Show("You lost.", "Server - Game Result", MessageBoxButtons.OK);
                                {
                                    if (res == DialogResult.Yes)
                                    {
                                        Environment.Exit(1);
                                    }
                                    else
                                        Environment.Exit(1);
                                }
                                return;
                            }
                        }

                        return;
                    }
                } else
                {
                    _extraRoundPublisher.Unsubscribe(_extraRoundSubscriberMap.GetExtraRoundSubscriber(extraSubscriberToGet));
                    myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");

                    AttackToEnemy("hit:" + shotButtonName);

                    deletingButton.buttonNames.Remove(shotButtonName);

                    return;
                }
            }

            //  If the target is not hit, only the background of the relevant button is changed and in the 2nd iteration, data is sent to read this part again.
            else
            {
                _extraRoundPublisher.Unsubscribe(_extraRoundSubscriberMap.GetExtraRoundSubscriber(extraSubscriberToGet));
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
            gameFacade.GetAttackSender().SendAttack(TextToSend);
            backgroundWorker2.CancelAsync();
        }

        //  Click events of all buttons on which attacks will be made are associated with this method. When clicked, it sends the name of the button to enemy.

        private void button_click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            if (clickedButton == null || !clickedButton.Enabled)
            {
                return;
            }

            AttackToEnemy(clickedButton.Name);
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

            if (isIceberg)
            {
                isIceberg = false;
                return;
            }

            SwitchGameButtonsEnabled();
        }

        //  The method to activate or deactivate the buttons on the attack board according to the attack order.
        private void SwitchGameButtonsEnabled()
        {
            if (weHaveReceivedExtraRound && !isIceberg)
            {
                foreach (var item in gameBoardButtons)
                {
                    if (!clickedButtons.Contains(item.Name))
                    {
                        item.Enabled = true;
                    }
                }

                weHaveReceivedExtraRound = false;

                labelAttackTurn.Text = "ATTACK";
                areEnabledButtons = true;

                return;
            }

            if (enemyReceivedExtraRound && !isIceberg)
            {
                foreach (var item in gameBoardButtons)
                {
                    item.Enabled = false;
                }
                labelAttackTurn.Text = "WAIT...";
                areEnabledButtons = false;

                enemyReceivedExtraRound = false;

                richTextBox1.AppendText("Enemy extra round\n");
                return;
            }

            if (areEnabledButtons)
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
                    if (!clickedButtons.Contains(item.Name))
                    {
                        item.Enabled = true;
                    }
                }

                labelAttackTurn.Text = hasRadarUse ? "RANDOM RADAR" : "ATTACK";
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

            var formBuilder = new FormBuilder();
            var formCreator = new FormCreator(formBuilder);
            var frm2 = formCreator.BuildLightForm();
            frm2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ISoundImplementation backgroudSoundImplementation = new BackgroundMusic();
            SoundPlayerBridge backgroundSoundPlayer = new BackgroundMusicPlayer(backgroudSoundImplementation);
            backgroundSoundPlayer.Play();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ISoundImplementation backgroudSoundImplementation = new BackgroundMusic();
            SoundPlayerBridge backgroundSoundPlayer = new BackgroundMusicPlayer(backgroudSoundImplementation);
            backgroundSoundPlayer.Stop();
        }
    }
}
