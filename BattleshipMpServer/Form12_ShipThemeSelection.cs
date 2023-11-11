using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleshipMpServer.Factory.Ship;

namespace BattleshipMp
{
    public partial class Form12_ShipThemeSelection : Form
    {
        public Form12_ShipThemeSelection()
        {
            InitializeComponent();
        }

        private void btnLightTheme_Click(object sender, EventArgs e)
        {
            Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen(new LightShipFactory());
            frm2.Show();
            this.Close();
        }

        private void btnDarkTheme_Click(object sender, EventArgs e)
        {
            Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen(new DarkShipFactory());
            frm2.Show();
            this.Close();
        }
    }
}