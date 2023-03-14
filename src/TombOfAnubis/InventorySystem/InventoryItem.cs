﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis.CollisionSystem;

namespace TombOfAnubis.InventorySystem
{
    enum ItemTypes
    {
        None,
        Speedup
    }

    internal class InventoryItem
    {
        ItemTypes itemType;
        public string Name { get; set; }
        public string Description { get; set; }

        public bool isInWorld = false;
        public bool isInInventory = false;
        public InventoryItem() { }

    }
}
