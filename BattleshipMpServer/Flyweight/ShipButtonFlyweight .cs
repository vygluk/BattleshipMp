using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Flyweight
{
    public class ShipButtonFlyweight : IShipButtonFlyweight
    {
        private readonly Color color;

        public ShipButtonFlyweight(Color color)
        {
            this.color = color;
        }

        public void DisplayShipButton(ShipButtonContext context)
        {
            context.Button.BackColor = this.color;
        }
    }
}
