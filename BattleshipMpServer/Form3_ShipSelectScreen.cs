using BattleshipMp.Builder;
using BattleshipMp.IteratorExtra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipMp
{
    public partial class Form3_ShipSelectScreen : Form
    {
        //  This class stores which ship will fit how many buttons in the Dictionary structure. Lists the ship that can be selected based on the number of buttons retrieved from Form2.

        List<string> buttonsNames;

        public string returnValue { get; set; }

        private HashSet<ShipSize> squarePerShips = new HashSet<ShipSize>
        {
            new ShipSize { Name = "Cruiser", Size = 3 },
            new ShipSize { Name = "Destroyer", Size = 2 },
            new ShipSize { Name = "Submarine", Size = 1 }
        };

        Dictionary<string, int> squarePerSpecialShips = new Dictionary<string, int>()
        {
            {"Battleship", 4}, {"SpecialCruiser", 3}, {"SpecialDestroyer", 2}, {"SpecialSubmarine", 1}
        };

        public Form3_ShipSelectScreen(List<string> buttonsNames)
        {
            this.buttonsNames = buttonsNames;
            InitializeComponent();
        }

        private bool AreAllShipsOfSquareSizePlaced(int squareSize)
        {
            int totalRemShips = 0;

            var shipAggregate = new ShipAggregate(Form2_PreparatoryScreen.shipList);
            var shipSizeAggregate = new ShipSizeAggregate(squarePerShips);

            var shipListIterator = shipAggregate.CreateIterator();
            var squarePerShipsIterator = shipSizeAggregate.CreateIterator();
            while (shipListIterator.HasNext())
            {
                var ship = shipListIterator.Next();

                while (squarePerShipsIterator.HasNext())
                {
                    var shipSize = squarePerShipsIterator.Next();
                    if (shipSize.Name == ship.shipName && shipSize.Size == squareSize)
                    {
                        totalRemShips += ship.remShips;
                    }
                }
            }

            foreach (var specialShip in Form2_PreparatoryScreen.specialShipList)
            {
                if (squarePerSpecialShips.ContainsKey(specialShip.shipName) && squarePerSpecialShips[specialShip.shipName] == squareSize)
                {
                    totalRemShips += specialShip.remShips;
                }
            }

            return totalRemShips <= 0;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            foreach (var item1 in Form2_PreparatoryScreen.shipList)
            {
                foreach (var item2 in squarePerShips)
                {
                    if (item1.shipName == item2.Name)
                    {
                        if (item1.remShips != 0 && item2.Size == buttonsNames.Count)
                        {
                            if (AreAllShipsOfSquareSizePlaced(item2.Size))
                            {
                                MessageBox.Show("There are no ships to place in the selected area.");
                                this.Close();
                                return;
                            }
                            listBox1.Items.Add(item1.shipName);
                        }
                    }
                }
                
            }

            foreach (var item1 in Form2_PreparatoryScreen.specialShipList)
            {
                foreach (var item2 in squarePerSpecialShips)
                {
                    if (item1.shipName == item2.Key)
                    {
                        if (item1.remShips != 0 && item2.Value == buttonsNames.Count)
                        {
                            if (AreAllShipsOfSquareSizePlaced(item2.Value))
                            {
                                MessageBox.Show("There are no ships to place in the selected area.");
                                this.Close();
                                return;
                            }
                            listBox1.Items.Add(item1.shipName);
                        }
                    }
                }

            }

            if (listBox1.Items.Count != 0)
            {
                listBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                this.returnValue = listBox1.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
