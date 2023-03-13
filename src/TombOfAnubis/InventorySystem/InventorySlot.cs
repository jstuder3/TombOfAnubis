using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.InventorySystem
{
    enum SlotType
    {
        ArtefactSlot,
        ItemSlot
    }
    internal class InventorySlot
    {

        public Item item { get; set; }
        //public int quantity { get; set; }
        public int slotNumber = -1;
        public SlotType slotType;
        public InventorySlot(int slotNumber, SlotType slotType)
        {
            this.slotNumber = slotNumber;
            this.slotType = slotType;
        }

        public void SetItem(Item item)
        {
            this.item = item;
        }

        public void ClearItem()
        {
            this.item = null;
        }
    }
}
