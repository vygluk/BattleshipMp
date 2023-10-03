﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMpClient.Factory.Ship
{
    public interface IShip
    {
        string shipName { get; }
        int remShips { get; set; }
        List<ShipButtons> shipPerButton { get; set; }
    }
}
