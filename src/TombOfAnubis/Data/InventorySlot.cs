using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public enum SlotType
    {
        ArtefactSlot,
        ItemSlot
    }
    public class InventorySlot
    {

        public InventoryItem item { get; set; }
        //public int quantity { get; set; }
        public int slotNumber = -1;
        public SlotType slotType;
        public InventorySlot(int slotNumber, SlotType slotType)
        {
            this.slotNumber = slotNumber;
            this.slotType = slotType;
            this.item = new InventoryItem();
        }

        public void SetItem(InventoryItem item)
        {
            this.item = item;
        }

        public InventoryItem GetItem()
        {
            return this.item;
        }

        public void ClearItem()
        {
            this.item.ItemType = ItemType.None;
        }
    }
}
