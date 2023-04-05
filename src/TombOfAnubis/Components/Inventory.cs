using System.Collections.Generic;
using TombOfAnubis;

namespace TombOfAnubis
{
    public class Inventory : Component
    {
        //public List<InventorySlot> InventorySlots { get; set; }
        
        public List<InventorySlot> ArtefactSlots { get; set; }
        public List<InventorySlot> ItemSlots { get; set; }

        public Inventory(int numArtefactSlots, int numItemSlots, Entity entity)
        {
            //InventorySlots = new List<InventorySlot>();
            ArtefactSlots = new List<InventorySlot>();
            ItemSlots = new List<InventorySlot>();

            //needs to be done here because otherwise we can't initialize the inventory slots before AddComponent()
            Entity = entity;

            for (int i = 0; i < numArtefactSlots; i++)
            {
                ArtefactSlots.Add(new InventorySlot(i, SlotType.ArtefactSlot, Entity));
            }

            //for now we only have one item slot
            for(int i = 0; i < 1; i++)
            {
                ItemSlots.Add(new InventorySlot(i, SlotType.ItemSlot, Entity));
            }

        }

        public void AddArtefact(int slotIndex = 0)
        {
            if (slotIndex < ArtefactSlots.Count)
            {
                InventorySlot slot = ArtefactSlots[slotIndex];
                if (slot.IsEmpty() && slot.SlotType == SlotType.ArtefactSlot)
                {
                    slot.Item = new InventoryItem(ItemType.Artefact, Entity);
                }
            }
        }

        public override void Delete()
        {
            base.Delete();
            ItemSlots = null;
        }

        public bool HasArtefact(int slotIndex = 0)
        {
            if (slotIndex > ArtefactSlots.Count) { return false; }
            if (!ArtefactSlots[slotIndex].IsEmpty() && ArtefactSlots[slotIndex].SlotType == SlotType.ArtefactSlot)
            {
                return true;
            }
            return false;
        }

        public void ClearArtefactSlots()
        {
            foreach (InventorySlot slot in ArtefactSlots)
            {
                if (!slot.IsEmpty() && slot.SlotType == SlotType.ArtefactSlot)
                {
                    slot.ClearItem();
                }
            }
        }

        public bool ArtefactSlotsFull()
        {
            foreach (InventorySlot slot in ArtefactSlots)
            {
                if (slot.IsEmpty() && slot.SlotType == SlotType.ArtefactSlot)
                {
                    return false;
                }
            }
            return true;

        }

        public int ArtefactCount()
        {
            int c = 0;
            foreach (InventorySlot slot in ArtefactSlots)
            {
                if (!slot.IsEmpty() && slot.SlotType == SlotType.ArtefactSlot)
                {
                    c++;
                }
            }
            return c;
        }

        public InventorySlot GetEmptySlotOfType(SlotType slotType)
        {
            if (slotType == SlotType.ArtefactSlot)
            {
                foreach (InventorySlot slot in ArtefactSlots)
                {
                    if (slot.SlotType == slotType && slot.IsEmpty()) return slot;
                }
            }
            else if(slotType == SlotType.ItemSlot)
            {
                foreach (InventorySlot slot in ItemSlots)
                {
                    if (slot.SlotType == slotType && slot.IsEmpty()) return slot;
                }
            }

            return null;
        }

        public InventorySlot GetEmptyItemSlot()
        {
            return GetEmptySlotOfType(SlotType.ItemSlot);
        }


        public bool HasResurrectItem()
        {
            return GetResurrectionSlot() != null;
        }

        public InventorySlot GetResurrectionSlot()
        {
            foreach(InventorySlot slot in ItemSlots)
            {
                if (slot.Item.ItemType == ItemType.Resurrection) return slot;
            }
            return null;
        }

        public InventorySlot GetFullItemSlot()
        {
            foreach (InventorySlot slot in ItemSlots)
            {
                if (slot.Item.ItemType != ItemType.None) return slot;
            }
            return null;
        }

    }
}
