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

            InventorySlots.Add(new InventorySlot(0, SlotType.ArtefactSlot));
            //AddArtefact();

        }

        public void AddArtefact()
        {
            InventorySlots[0].SetItem(new InventoryItem(ItemType.Artefact));
        }

        public override void Delete()
        {
            base.Delete();
            InventorySlots = null;
        }

        public bool HasArtefact()
        {
            return InventorySlots[0].GetItem().ItemType == ItemType.Artefact;
        }

    }
}
