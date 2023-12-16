using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using BattleshipMpClient.Entity;
using BattleshipMp.Builder;
using BattleshipMpClient.Factory.Ship;
using BattleshipMpClient.State;
using BattleshipMpClient.Flyweight;
using System.Diagnostics;
using System.Threading;
using BattleshipMpClient.Template;
using BattleshipMp.IteratorExtra;

namespace BattleshipMpClient
{
    public partial class Form2_PreparatoryScreen : Form
    {
        private IShipFactory _shipFactory;
        private IShipBuilder _builder;
        private ShipsCreator _shipsCreator;
        private ShipButtonFlyweightFactory flyweightFactory = new ShipButtonFlyweightFactory();
        private PreparationMode preparationMode;
        private Control[] controls;

        public Form2_PreparatoryScreen()
        {
            InitializeComponent();
            DoubleBuffered = true;
            controls = new Control[Controls.Count];
            for (int i = 0; i < Controls.Count; i++)
            {
                controls[i] = Controls[i];
            }
            //SetObsticlesUp();
        }

        public void SetPreparationMode(bool isClassicMode, int timeLimitInSeconds = 0)
        {
            if (isClassicMode)
            {
                preparationMode = new ClassicPreparationMode(this);
            }
            else
            {
                preparationMode = new TimedPreparationMode(this, timeLimitInSeconds);
            }
        }

        private void SetButtonProperties(Button button, Color color)
        {
            var flyweight = flyweightFactory.GetFlyweight(color);
            flyweight.DisplayShipButton(new ShipButtonContext { Button = button });
        }

        public void AddShipFactory(IShipFactory shipFactory)
        {
            _shipFactory = shipFactory;
            _builder = new ShipBuilder(_shipFactory);
            _shipsCreator = new ShipsCreator(_builder);
        }

        #region ****************************************************** 2D Drawing ******************************************************
        private Point selectionStart;
        private Point selectionEnd;
        private Rectangle selection;
        private bool mouseDown;
        public List<ShipButtons> icebergTiles = new List<ShipButtons>();
        public List<Control> icebergButtons = new List<Control>();
        public Iceberg iceberg = new Iceberg();
  


        protected override void OnMouseDown(MouseEventArgs e)
        {
            selectionStart = PointToClient(MousePosition);
            mouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseDown = false;

            SetSelectionRect();
            Invalidate();

            GetSelectedButtons();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!mouseDown)
            {
                return;
            }

            selectionEnd = PointToClient(MousePosition);
            SetSelectionRect();

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (mouseDown)
            {
                using (Pen pen = new Pen(Color.Black, 1F))
                {
                    pen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, selection);
                }
            }
        }

        public void SetObsticlesUp()
        {
            foreach(Control c in Controls)
            {
                if (c is Button)
                {
                    if (c.Name == "E4")
                    {
                        c.BackColor = Color.Blue;
                        icebergButtons.Add(c);
                    }
                    if (c.Name == "F4")
                    {
                        c.BackColor = Color.Blue;
                        icebergButtons.Add(c);
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
            iceberg.ReplaceTiles(icebergTiles);
        }

        private void SetSelectionRect()
        {
            int x, y;
            int width, height;

            x = selectionStart.X > selectionEnd.X ? selectionEnd.X : selectionStart.X;
            y = selectionStart.Y > selectionEnd.Y ? selectionEnd.Y : selectionStart.Y;

            width = selectionStart.X > selectionEnd.X ? selectionStart.X - selectionEnd.X : selectionEnd.X - selectionStart.X;
            height = selectionStart.Y > selectionEnd.Y ? selectionStart.Y - selectionEnd.Y : selectionEnd.Y - selectionStart.Y;

            selection = new Rectangle(x, y, width, height);
        }

        #endregion


        public static List<IShip> shipList = new List<IShip>();
        public static List<ISpecialShip> specialShipList = new List<ISpecialShip>();
        public static ShipComposite allShipsComposite = new ShipComposite("Fleet");

        List<(string, Color)> AllSelectedButtonList = new List<(string, Color)>();

        bool isPanelActive = true;

        private void Form2_Load(object sender, EventArgs e)
        {
            shipList = null;
            //CreateShipList();
            //RemainingShips();
            preparationMode.PrepareBoard();
            timer1.Start();
            //TestFlyweight();
        }

        private void TestFlyweight()
        {
            PrepareForMemoryMeasurement();

            Stopwatch stopwatch = new Stopwatch();

            // Testing Without Flyweight
            stopwatch.Start();
            long memoryWithoutFlyweight = WithoutFlyweightTest();
            stopwatch.Stop();
            long timeWithoutFlyweight = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Memory usage without Flyweight: {memoryWithoutFlyweight} bytes");
            Console.WriteLine($"Time taken without Flyweight: {timeWithoutFlyweight} ms");

            PrepareForMemoryMeasurement();

            // Testing With Flyweight
            stopwatch.Restart();
            long memoryWithFlyweight = WithFlyweightTest();
            stopwatch.Stop();
            long timeWithFlyweight = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Memory usage with Flyweight: {memoryWithFlyweight} bytes");
            Console.WriteLine($"Time taken with Flyweight: {timeWithFlyweight} ms");
        }

        private void PrepareForMemoryMeasurement()
        {
            // This ensures that it's a fair test
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private List<Button> CreateButtons(int v)
        {
            var buttons = new List<Button>();
            for (int i = 0; i < v; i++)
            {
                var button = new Button();
                buttons.Add(button);
            }
            return buttons;
        }

        private long WithoutFlyweightTest()
        {
            PrepareForMemoryMeasurement();
            long memoryBefore = GC.GetTotalMemory(true);

            List<Button> buttons = CreateButtons(1000000); // Create 1 million buttons

            foreach (var button in buttons)
            {
                button.BackColor = Color.FromArgb(255, 0, 0); // Example color
                                                              // ... other property settings
            }

            long memoryAfter = GC.GetTotalMemory(true);
            return memoryAfter - memoryBefore;
        }

        private long WithFlyweightTest()
        {
            PrepareForMemoryMeasurement();
            long memoryBefore = GC.GetTotalMemory(true);

            List<Button> buttons = CreateButtons(1000000); // Create 1 million buttons
            Color color = Color.FromArgb(255, 0, 0); // Example color

            foreach (var button in buttons)
            {
                // Using Flyweight to set properties
                SetButtonProperties(button, color);
            }

            long memoryAfter = GC.GetTotalMemory(true);
            return memoryAfter - memoryBefore;
        }


        // 1 // In the first step, create ships derived from the "Ship" model (class) and list these ships.
        public void CreateShipList()
        {
            shipList = _shipsCreator.BuildNormalShips();
            specialShipList = _shipsCreator.BuildSpecialShips();

            ShipComposite normalShipsSquadron = new ShipComposite("NormalSquadron");
            ShipComposite specialShipsSquadron = new ShipComposite("SpecialSquadron");

            foreach (var ship in shipList)
            {
                normalShipsSquadron.Add(ship);
            }

            foreach (var specialShip in specialShipList)
            {
                specialShipsSquadron.Add(specialShip);
            }

            allShipsComposite.Add(normalShipsSquadron);
            allShipsComposite.Add(specialShipsSquadron);
        }
        private bool ValidateTile(Control button)
        {
            foreach (Control obsticle in icebergButtons)
            {
                if (obsticle.Name == button.Name)
                {
                    return true;
                }
            }

            return false;
        }

        private void ValidateSelection()
        {
            var controlsIterator = new ControlsIterator(controls);
            while(controlsIterator.HasNext())
            {
                var c = controlsIterator.Next();

                if (c is Button && selection.IntersectsWith(c.Bounds) && ValidateTile(c))
                {
                    MessageBox.Show("Can't create ship over obsticle.");
                    selection = new Rectangle();

                    return;
                }
            }
        }

        private void GetSelectedButtons()
        {
            ValidateSelection();
            List<Button> selected = new List<Button>();

            foreach (Control c in Controls)
            {
                if (c is Button)
                {
                    if (selection.IntersectsWith(c.Bounds))
                    {
                        selected.Add((Button)c);
                    }
                }
            }

            List<string> buttonsNames = new List<string>();

            foreach (Control c in selected)
            {
                buttonsNames.Add(c.Name);
            }

            buttonsNames.Sort();

            List<int> totalAsciiList = new List<int>();
            foreach (var item in buttonsNames)
            {
                char ch1 = char.Parse(item.Substring(0, 1));
                char ch2 = char.Parse(item.Substring(item.Length - 1));
                totalAsciiList.Add((int)ch1 + (int)ch2);
            }

            if (totalAsciiList.Count > 1)
            {
                for (int i = 0; i < totalAsciiList.Count - 1; i++)
                {
                    if (totalAsciiList[i] + 1 != totalAsciiList[i + 1])
                    {
                        MessageBox.Show("The area where the ship will be located must be consecutive boxes.");
                        return;
                    }
                }
            }

            foreach (var item in selected)
            {
                foreach (var ship in shipList)
                    if (item.BackColor == ship.color)
                    {
                        DeleteShip(selected);
                        return;
                    }
                foreach (var ship in specialShipList)
                    if (item.BackColor == ship.color)
                    {
                        DeleteShip(selected);
                        return;
                    }
            }

            if (selected.Count > 0 && selected.Contains(buttonStart) == false)
            {
                using (var frm3 = new Form3_ShipSelectScreen(buttonsNames))
                {
                    var res = frm3.ShowDialog();
                    if (res == DialogResult.OK)
                    {
                        string val = frm3.returnValue;
                        foreach (var item in shipList)
                        {
                            if (item.shipName == val)
                            {
                                item.remShips--;

                                if (item.shipPerButton == null)
                                    item.shipPerButton = new List<ShipButtons>();

                                ShipButtons sb = new ShipButtons() { buttonNames = new List<string>() };
                                foreach (var item2 in selected)
                                {
                                    SetButtonProperties(item2, item.color);
                                    sb.buttonNames.Add(item2.Name);
                                }
                                item.shipPerButton.Add(sb);
                                RemainingShips();
                                break;
                            }
                        }

                        foreach (var item in specialShipList)
                        {
                            if (item.shipName == val)
                            {
                                item.remShips--;

                                if (item.shipPerButton == null)
                                    item.shipPerButton = new List<ShipButtons>();

                                ShipButtons sb = new ShipButtons() { buttonNames = new List<string>() };
                                foreach (var item2 in selected)
                                {
                                    item2.BackColor = item.color;
                                    sb.buttonNames.Add(item2.Name);
                                }
                                item.shipPerButton.Add(sb);

                                RemainingShips();
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void DeleteShip(List<Button> selected)
        {
            DialogResult dres = MessageBox.Show("Are you sure you want to delete the ship?", "Delete Ship", MessageBoxButtons.YesNo);
            if (dres == DialogResult.Yes)
            {
                foreach (var item1 in shipList)
                {
                    if (item1.shipPerButton != null)
                    {
                        foreach (var item2 in item1.shipPerButton)
                        {
                            foreach (var item3 in selected)
                            {
                                if (item2.buttonNames.FirstOrDefault(x => x.Equals(item3.Name)) != null)
                                {
                                    item1.remShips++;
                                    foreach (var item4 in item2.buttonNames)
                                    {
                                        this.Controls.Find(item4, true)[0].BackColor = Color.Transparent;
                                    }
                                    item1.shipPerButton.Remove(item2);
                                    RemainingShips();
                                    return;
                                }
                            }
                        }
                    }
                }

                foreach (var item1 in specialShipList)
                {
                    if (item1.shipPerButton != null)
                    {
                        foreach (var item2 in item1.shipPerButton)
                        {
                            foreach (var item3 in selected)
                            {
                                if (item2.buttonNames.FirstOrDefault(x => x.Equals(item3.Name)) != null)
                                {
                                    item1.remShips++;
                                    foreach (var item4 in item2.buttonNames)
                                    {
                                        this.Controls.Find(item4, true)[0].BackColor = Color.Transparent;
                                    }
                                    item1.shipPerButton.Remove(item2);
                                    RemainingShips();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }


        private void yardımToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1- Hold down the mouse LC and select the location where you want to place the ships.\n\n" +
                "2- \r\nSelect from the page listing the ships that can be placed in the selected location.\n\n" +
                "3- Click 'Ready' when you have placed all your ships.\n\n" +
                "Notice: The game will start after both players 'Continue'.");
        }

        public void RemainingShips()
        {
            lblBattleship.Text = lblBattleship.Text.Substring(0, lblBattleship.Text.Length - 1);
            lblBattleship.Text += specialShipList.FirstOrDefault(x => x.shipName == "Battleship").remShips.ToString();

            lblCruiser.Text = lblCruiser.Text.Substring(0, lblCruiser.Text.Length - 1);
            lblCruiser.Text += shipList.FirstOrDefault(x => x.shipName == "Cruiser").remShips.ToString();

            lblDestroyer.Text = lblDestroyer.Text.Substring(0, lblDestroyer.Text.Length - 1);
            lblDestroyer.Text += shipList.FirstOrDefault(x => x.shipName == "Destroyer").remShips.ToString();

            lblSubmarine.Text = lblSubmarine.Text.Substring(0, lblSubmarine.Text.Length - 1);
            lblSubmarine.Text += shipList.FirstOrDefault(x => x.shipName == "Submarine").remShips.ToString();

            lblSpecialSubmarine.Text = lblSpecialSubmarine.Text.Substring(0, lblSpecialSubmarine.Text.Length - 1);
            lblSpecialSubmarine.Text += specialShipList.FirstOrDefault(x => x.shipName == "SpecialSubmarine").remShips.ToString();

            lblSpecialCruiser.Text = lblSpecialCruiser.Text.Substring(0, lblSpecialCruiser.Text.Length - 1);
            lblSpecialCruiser.Text += specialShipList.FirstOrDefault(x => x.shipName == "SpecialCruiser").remShips.ToString();

            lblSpecialDestroyer.Text = lblSpecialDestroyer.Text.Substring(0, lblSpecialDestroyer.Text.Length - 1);
            lblSpecialDestroyer.Text += specialShipList.FirstOrDefault(x => x.shipName == "SpecialDestroyer").remShips.ToString();

            isPanelActive = true;

            foreach (var item in shipList)
            {
                if (item.remShips != 0)
                {
                    isPanelActive = false;
                    break;
                }
            }
            if (isPanelActive)
            {
                foreach (var item in specialShipList)
                {
                    if (item.remShips != 0)
                    {
                        isPanelActive = false;
                    }
                }
            }
            if (isPanelActive == true)
            {
                buttonStart.Enabled = true;
                buttonStart.Text = "Ready.\nStart";
            }
            else
            {
                buttonStart.Enabled = false;
                buttonStart.Text = "Preparatory";
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            
            GameContext gameContext = GameContext.Instance;
            var ships = FillAllButtonList();
            gameContext.SetTheme(ships);
            PlayerTurnState playerTurnState = PlayerTurnState.Instance;
            gameContext.TransitionTo(playerTurnState);
            this.Visible = false;
        }

        private List<(string, Color)> FillAllButtonList()
        {
            foreach (var item1 in shipList)
            {
                foreach (var item2 in item1.shipPerButton)
                {
                    foreach (var item3 in item2.buttonNames)
                    {
                        AllSelectedButtonList.Add((item3, item1.color));
                    }
                }
            }
            foreach (var item1 in specialShipList)
            {
                foreach (var item2 in item1.shipPerButton)
                {
                    foreach (var item3 in item2.buttonNames)
                    {
                        AllSelectedButtonList.Add((item3, item1.color));
                    }
                }
            }
            return AllSelectedButtonList;
        }

        public void UpdateCountdownDisplay(int remainingSeconds)
        {
            countdownLabel.Text = $"Time left: {remainingSeconds} seconds";
        }

        public void EndPreparation()
        {
            AutomaticallyPlaceUnplacedShips();
            buttonStart_Click(null, EventArgs.Empty);
        }

        private void AutomaticallyPlaceUnplacedShips()
        {
            var occupiedButtons = FillAllButtonList().Select(item => item.Item1).ToHashSet();

            foreach (var ship in shipList)
            {
                while (ship.remShips > 0)
                {
                    var selectedButtonNames = FindValidPlacementForShip(ship, occupiedButtons);
                    if (selectedButtonNames != null)
                    {
                        PlaceShip(ship, selectedButtonNames);
                        ship.remShips--;
                        foreach (var buttonName in selectedButtonNames)
                        {
                            occupiedButtons.Add(buttonName);
                        }
                    }
                }
            }

            foreach (var specialShip in specialShipList)
            {
                while (specialShip.remShips > 0)
                {
                    var selectedButtonNames = FindValidPlacementForSpecialShip(specialShip, occupiedButtons);
                    if (selectedButtonNames != null)
                    {
                        PlaceSpecialShip(specialShip, selectedButtonNames);
                        specialShip.remShips--;
                        foreach (var buttonName in selectedButtonNames)
                        {
                            occupiedButtons.Add(buttonName);
                        }
                    }
                }
            }
        }
        private List<(int, int)> GenerateShuffledGridPositions()
        {
            Random _random = new Random();

            var positions = new List<(int, int)>();
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    positions.Add((row, col));
                }
            }

            int n = positions.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                var value = positions[k];
                positions[k] = positions[n];
                positions[n] = value;
            }

            return positions;
        }

        private List<string> FindValidPlacementForShip(IShip ship, HashSet<string> occupiedButtons)
        {
            int shipSize = GetShipSize(ship);
            var shuffledPositions = GenerateShuffledGridPositions();
            var ignoredPositions = new HashSet<string> { "F4", "E4" };

            foreach (var (row, col) in shuffledPositions)
            {
                string position = $"{row}{col}";
                if (ignoredPositions.Contains(position))
                    continue;

                var horizontalButtons = GetConsecutiveButtonNames(row, col, shipSize, true, occupiedButtons);
                if (horizontalButtons != null)
                    return horizontalButtons;

                var verticalButtons = GetConsecutiveButtonNames(row, col, shipSize, false, occupiedButtons);
                if (verticalButtons != null)
                    return verticalButtons;
            }

            return null;
        }

        private List<string> FindValidPlacementForSpecialShip(ISpecialShip ship, HashSet<string> occupiedButtons)
        {
            int shipSize = GetSpecialShipSize(ship);
            var shuffledPositions = GenerateShuffledGridPositions();
            var ignoredPositions = new HashSet<string> { "F4", "E4" };

            foreach (var (row, col) in shuffledPositions)
            {
                string position = $"{row}{col}";
                if (ignoredPositions.Contains(position))
                    continue;

                var horizontalButtons = GetConsecutiveButtonNames(row, col, shipSize, true, occupiedButtons);
                if (horizontalButtons != null)
                    return horizontalButtons;

                var verticalButtons = GetConsecutiveButtonNames(row, col, shipSize, false, occupiedButtons);
                if (verticalButtons != null)
                    return verticalButtons;
            }

            return null;
        }

        private int GetShipSize(IShip ship)
        {
            Dictionary<string, int> squarePerShips = new Dictionary<string, int>()
            {
                {"Cruiser", 3}, {"Destroyer", 2}, {"Submarine", 1}
            };

            if (squarePerShips.ContainsKey(ship.shipName))
                return squarePerShips[ship.shipName];
            return 0;
        }

        private int GetSpecialShipSize(ISpecialShip ship)
        {
            Dictionary<string, int> squarePerSpecialShips = new Dictionary<string, int>()
            {
                {"Battleship", 4}, {"SpecialCruiser", 3}, {"SpecialDestroyer", 2}, {"SpecialSubmarine", 1}
            };

            if (squarePerSpecialShips.ContainsKey(ship.shipName))
                return squarePerSpecialShips[ship.shipName];
            return 0;
        }

        private List<string> GetConsecutiveButtonNames(int startRow, int startCol, int shipSize, bool horizontal, HashSet<string> occupiedButtons)
        {
            List<string> buttonNames = new List<string>();
            var excludedPositions = new HashSet<string> { "F4", "E4" };

            for (int i = 0; i < shipSize; i++)
            {
                int row = horizontal ? startRow : startRow + i;
                int col = horizontal ? startCol + i : startCol;

                string buttonName = GetButtonNameAtPosition(row, col);

                if (buttonName == null || occupiedButtons.Contains(buttonName) || excludedPositions.Contains(buttonName))
                    return null;

                buttonNames.Add(buttonName);
            }

            return buttonNames;
        }

        private string GetButtonNameAtPosition(int row, int col)
        {
            if (row < 0 || row >= 10 || col < 0 || col >= 10)
                return null;

            char rowLetter = (char)('A' + row);
            return $"{rowLetter}{col}";
        }

        private void PlaceShip(IShip ship, List<string> buttonNames)
        {
            if (ship.shipPerButton == null)
                ship.shipPerButton = new List<ShipButtons>();

            ShipButtons sb = new ShipButtons() { buttonNames = new List<string>() };
            foreach (var buttonName in buttonNames)
            {
                var button = this.Controls.Find(buttonName, true).FirstOrDefault() as Button;
                if (button != null)
                {
                    button.BackColor = ship.color;
                    sb.buttonNames.Add(buttonName);
                }
            }
            ship.shipPerButton.Add(sb);
            RemainingShips();
        }

        private void PlaceSpecialShip(ISpecialShip ship, List<string> buttonNames)
        {
            if (ship.shipPerButton == null)
                ship.shipPerButton = new List<ShipButtons>();

            ShipButtons sb = new ShipButtons() { buttonNames = new List<string>() };
            foreach (var buttonName in buttonNames)
            {
                var button = this.Controls.Find(buttonName, true).FirstOrDefault() as Button;
                if (button != null)
                {
                    button.BackColor = ship.color;
                    sb.buttonNames.Add(buttonName);
                }
            }
            ship.shipPerButton.Add(sb);
            RemainingShips();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Client.GetInstance.IsConnected)
            {
                MessageBox.Show("Connection failed.");
                Client.GetInstance.CloseAndDispose();
                timer1.Stop();
                this.Close();
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
            Form1_ClientScreen frm1 = new Form1_ClientScreen();
            frm1.Visible = true;
        }
    }
}
