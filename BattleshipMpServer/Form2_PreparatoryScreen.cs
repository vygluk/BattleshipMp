using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleshipMpServer.Factory.Ship;

namespace BattleshipMp
{
    public partial class Form2_PreparatoryScreen : Form
    {
        private readonly IShipFactory _shipFactory;

        public Form2_PreparatoryScreen(IShipFactory shipFactory)
        {
            _shipFactory = shipFactory;
            InitializeComponent();
            DoubleBuffered = true;            
        }

        //  Create a drawing to select ship positions starting with a mouse click. Get selected buttons when mouse is released.
        //  This region also includes the GetSelectedButtons() method. However, since other operations are done in this method, I did not include it in the region.
        #region ****************************************************** 2D Drawing ******************************************************
        private Point selectionStart;
        private Point selectionEnd;
        private Rectangle selection;
        private bool mouseDown;


        
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

        List<(string, Color)> AllSelectedButtonList = new List<(string, Color)>();

        bool isPanelActive = true;


        private void Form2_Load(object sender, EventArgs e)
        {
            timer1.Start();
            shipList = null;
            CreateShipList();
            RemainingShips();
        }

        // 1 // In the first step, create ships derived from the "Ship" model (class) and list these ships.
        private void CreateShipList()
        {
            if (shipList == null)
            {
                shipList = new List<IShip>();
            }

            IShip submarine = _shipFactory.CreateSubmarine();
            IShip destroyer = _shipFactory.CreateDestroyer();
            IShip cruiser = _shipFactory.CreateCruiser();
            IShip battleship = _shipFactory.CreateBattleship();
            shipList.Add(submarine);
            shipList.Add(destroyer);
            shipList.Add(cruiser);
            shipList.Add(battleship);
        }

        private void GetSelectedButtons()
        {
            // 2 // Put the buttons selected with the mouse into the list.

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

            // 3 // After the selected buttons are put in the list and sorted alphabetically. Check if the selected boxes are consecutive.
            // After separating the characters of the box names and adding the ascii values ​​one by one, make sure that there is 1 difference between them and the next box.

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

            // 4 //  If the color of any of the selected buttons is "DarkGray" i.e. it contains part of a predefined ship.
            // Call the "DeleteShip" method.
            foreach (var item in selected)
            {
                foreach (var ship in shipList)
                    if (item.BackColor == ship.color)
                    {
                        DeleteShip(selected);
                        return;
                    }
            }

            // 6 // If the buttons are selected for the first time, bring up Form3 as a dialog window listing the ship that matches the number of buttons selected.
            // 7 // If it returns with a "Save" result, save the returning ship and related buttons according to the Ship model. + Decrease "remShip" in model.
            //  Call the RemainingShip() method after registration.
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

        // 5 // If the deletion is confirmed, find which ship the button belongs to and delete that ship from the list. + Increase "remShip" in the model.
        // After saving, call the RemainingShip() method.
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
            }
        }

        // 8 // Print the number of ships that need to be added according to the ship list. If there are no missing ships, activate the Ready/Start button.
        private void RemainingShips()
        {
            //{"Battleship", 1 },{"Cruiser", 2},{"Destroyer", 3},{"Submarine", 4}
            lblBattleship.Text = lblBattleship.Text.Substring(0, lblBattleship.Text.Length - 1);
            lblBattleship.Text += shipList.FirstOrDefault(x => x.shipName == "Battleship").remShips.ToString();

            lblCruiser.Text = lblCruiser.Text.Substring(0, lblCruiser.Text.Length - 1);
            lblCruiser.Text += shipList.FirstOrDefault(x => x.shipName == "Cruiser").remShips.ToString();

            lblDestroyer.Text = lblDestroyer.Text.Substring(0, lblDestroyer.Text.Length - 1);
            lblDestroyer.Text += shipList.FirstOrDefault(x => x.shipName == "Destroyer").remShips.ToString();

            lblSubmarine.Text = lblSubmarine.Text.Substring(0, lblSubmarine.Text.Length - 1);
            lblSubmarine.Text += shipList.FirstOrDefault(x => x.shipName == "Submarine").remShips.ToString();

            foreach (var item in shipList)
            {
                if (item.remShips == 0)
                {
                    isPanelActive = true;
                }
                else
                {
                    isPanelActive = false;
                    break;
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

        private void yardımToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1- Hold down the mouse LC and select the location where you want to place the ships.\n\n" +
                "2- \r\nSelect from the page listing the ships that can be placed in the selected location.\n\n" +
                "3- Click 'Ready' when you have placed all your ships.\n\n" +
                "Notice: The game will start after both players 'Continue'.");
        }

        
        //  Oyun ekranına geçerken oyuncunun kendi gemilerini görebilmesi için constructor methoda seçilen gemileri içeren buton listesini gönder.
        private void buttonStart_Click(object sender, EventArgs e)
        {
            timer1.Stop();

            Form4_GameScreen frm4 = new Form4_GameScreen(FillAllButtonList());
            this.Visible = false;
            frm4.Show();
        }

        private List<(string, Color)> FillAllButtonList()
        {
            foreach (var item1 in shipList)
            {
                foreach(var item2 in item1.shipPerButton)
                {
                    foreach (var item3 in item2.buttonNames)
                    {
                        AllSelectedButtonList.Add((item3, item1.color));
                    }
                }
            }
            return AllSelectedButtonList;
        }

        //  Check every 1 second if the client is disconnected.
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Server.GetInstance.IsListenerActive != null && !Server.GetInstance.IsClientConnected)
            {
                MessageBox.Show("Client connection failed.");
            }
        }

        private void Form2_PreparatoryScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
            Form1_ServerScreen frm1 = new Form1_ServerScreen();
            frm1.Show();
        }
    }
}
