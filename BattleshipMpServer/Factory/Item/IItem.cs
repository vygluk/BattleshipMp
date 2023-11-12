﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipMp.Factory.Item
{
    public interface IItem
    {
        string itemName { get; }
        int remItems { get; set; }

        string FindRandomShip();
    }
}
