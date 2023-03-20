using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Inventory : Component
    {
        public List<InventorySlot> InventorySlots {  get; set; }

        public Inventory() {
            InventorySlots = new List<InventorySlot>();
        }
    }
}
