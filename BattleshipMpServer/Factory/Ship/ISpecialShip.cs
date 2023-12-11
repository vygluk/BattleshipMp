﻿using BattleshipMpServer.Factory.Ship;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpServer.Factory.Ship
{
    public interface ISpecialShip : IShipComponent
    {
        string shipName { get; }
        int remShips { get; set; }
        List<ShipButtons> shipPerButton { get; set; }
        Color color { get; set; }
        int remShields { get; set; }
    }
}
