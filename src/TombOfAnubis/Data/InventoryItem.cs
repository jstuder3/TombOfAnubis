using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public enum ItemType
    {
        None,
        Speedup
    }

    public class InventoryItem
    {
        public ItemType ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool isInWorld = false;
        public bool isInInventory = false;
        public InventoryItem() { }

    }
}
