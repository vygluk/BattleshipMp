using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BattleshipMpClient.Factory.Ship;
using BattleshipMpClient.Facade;
using BattleshipMpClient.Entity;
using BattleshipMpClient.Strategy;
using BattleshipMpClient.Observer;
using BattleshipMpClient.Bridge.Abstraction;
using BattleshipMpClient.Bridge.Concrete;
using BattleshipMp.Builder;
using BattleshipMpClient.Adapter;

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
        private readonly IFormBuilder _formBuilder;
        private readonly FormCreator _formCreator;
        bool isIceberg = false;
        bool skipIcebergChange = false;
        int turns = 0;

        public Form4_GameScreen(List<(string, Color)> list)
        {
            InitializeComponent(); 
            this.AllSelectedButtonList = list;
            Control.CheckForIllegalCrossThreadCalls = false;

            _formBuilder = new FormBuilder();
            _formCreator = new FormCreator(_formBuilder);
            _radarStrategyGenerator = new RadarStrategyGenerator();
            _extraRoundSubscriberMap = new ExtraRoundSubscriberMap();
            _extraRoundPublisher = new ExtraRoundPublisher();

            foreach (var subscriber in _extraRoundSubscriberMap.GetSubscribers())
            {
                _extraRoundPublisher.Subscribe(subscriber);
            }

            gameFacade = new GameFacade();

        }

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
                gameFacade.GetGameCommunication().StartGameCommunication();
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
                    string recieve = gameFacade.GetAttackReceiver().ReceiveAttack();
                    if (!string.IsNullOrEmpty(recieve))
                    {
                        AttackFromEnemy(recieve);
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

            //gameFacade.SendAttack(TextToSend);
            gameFacade.GetAttackSender().SendAttack(TextToSend);
            backgroundWorker2.CancelAsync();
        }

        private void button_click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            if (clickedButton == null || !clickedButton.Enabled)
            {
                return;
            }

            AttackToEnemy(clickedButton.Name);
        }

        public void AttackFromEnemy(string recieve)
        {
            ISoundImplementation hitSoundImplementation = new HitSound();
            ISoundImplementation missSoundImplementation = new MissSound();
            SoundPlayerBridge hitSoundPlayer = new HitSoundPlayer(hitSoundImplementation);
            SoundPlayerBridge missSoundPlayer = new MissSoundPlayer(missSoundImplementation);
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
                richTextBox1.AppendText("Hit shield\n");
                hitSoundPlayer.Play();
                return;
            }
            else if (recieve.Contains("youwin"))
            {
                DialogResult res = MessageBox.Show("Victory.", "Client - Game Result", MessageBoxButtons.OK);
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
                richTextBox1.AppendText($"[Lucky] The block you selected gave you extra round!\n");
                SwitchGameButtonsEnabled();
                return;
            } 
            else if (recieve.Contains("iceberg"))
            {
                isIceberg = true;
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

            bool hasShield = false;
            bool isShot = false;
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
                            else
                            {
                                hasShield = false;
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
                        foreach (var item in Form2_PreparatoryScreen.shipList.FirstOrDefault(x => x.shipName == "Battleship").shipPerButton)
                        {
                            if (item.buttonNames.Count > 0)
                            {
                                return;
                            }
                            AttackToEnemy("youwin");
                            DialogResult res = MessageBox.Show("You lost.", "Client - Game Result", MessageBoxButtons.OK);
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
                }
            }
            else
            {
                if (myBoardButtons == null)
                {
                    return;
                }

                _extraRoundPublisher.Unsubscribe(_extraRoundSubscriberMap.GetExtraRoundSubscriber(extraSubscriberToGet));
                myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                AttackToEnemy("miss:" + shotButtonName);
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

            if (isIceberg)
            {
                isIceberg = false;
                return;
            }

            SwitchGameButtonsEnabled();
        }

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

                labelAttackTurn.Text = "ATTACK";
                areEnabledButtons = true;
                weHaveReceivedExtraRound = false;

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
            if (!Client.GetInstance.IsConnected)
            {
                MessageBox.Show("Connection failed.");
                Form2_PreparatoryScreen frm2 = _formCreator.BuildDarkForm();
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
            Form2_PreparatoryScreen frm2 = _formCreator.BuildDarkForm();
            frm2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ISoundImplementation backgroudSoundImplementation = new BackgroundMusic();
            SoundPlayerBridge backgroundSoundPlayer = new BackgroundMusicPlayer(backgroudSoundImplementation);
            backgroundSoundPlayer.Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ISoundImplementation backgroudSoundImplementation = new BackgroundMusic();
            SoundPlayerBridge backgroundSoundPlayer = new BackgroundMusicPlayer(backgroudSoundImplementation);
            backgroundSoundPlayer.Stop();
        }
    }
}
