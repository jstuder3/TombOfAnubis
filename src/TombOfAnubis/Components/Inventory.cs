using System.Collections.Generic;

namespace TombOfAnubis
{
    public class Inventory : Component
    {
        public List<InventorySlot> InventorySlots { get; set; }

        public Inventory(int numArtefactSlots, int numItemSlots)
        {
            InventorySlots = new List<InventorySlot>();

            for (int i = 0; i < numArtefactSlots; i++)
            {
                InventorySlots.Add(new InventorySlot(i, SlotType.ArtefactSlot));
            }
            //with the current version, we always have one item slot per SlotType
            /*for (int i = numArtefactSlots; i < numArtefactSlots + numItemSlots; i++)
            {
                InventorySlots.Add(new InventorySlot(i, SlotType.ItemSlot));
            }*/

            InventorySlots.Add(new InventorySlot(numArtefactSlots, SlotType.BodyPowerupSlot));
            InventorySlots.Add(new InventorySlot(numArtefactSlots + 1, SlotType.WisdomPowerupSlot));
            InventorySlots.Add(new InventorySlot(numArtefactSlots + 2, SlotType.ResurrectionSlot));

        }

        public void AddArtefact(int slotIndex = 0)
        {
            if (slotIndex < InventorySlots.Count)
            {
                InventorySlot slot = InventorySlots[slotIndex];
                if (slot.IsEmpty() && slot.SlotType == SlotType.ArtefactSlot)
                {
                    slot.Item = new InventoryItem(ItemType.Artefact);
                }
            }
        }

        public override void Delete()
        {
            base.Delete();
            InventorySlots = null;
        }

        public bool HasArtefact(int slotIndex = 0)
        {
            if (slotIndex > InventorySlots.Count) { return false; }
            if (!InventorySlots[slotIndex].IsEmpty() && InventorySlots[slotIndex].SlotType == SlotType.ArtefactSlot)
            {
                return true;
            }
            return false;
        }

        public void ClearArtefactSlots()
        {
            foreach (InventorySlot slot in InventorySlots)
            {
                if (!slot.IsEmpty() && slot.SlotType == SlotType.ArtefactSlot)
                {
                    slot.ClearItem();
                }
            }
        }

        public bool ArtefactSlotsFull()
        {
            foreach (InventorySlot slot in InventorySlots)
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
            foreach (InventorySlot slot in InventorySlots)
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
            foreach(InventorySlot slot in InventorySlots)
            {
                if (slot.SlotType == slotType && slot.IsEmpty()) return slot;
            }
            return null;
        }

        public InventorySlot GetEmptyBodyPowerupSlot()
        {
            return GetEmptySlotOfType(SlotType.BodyPowerupSlot);
        }

        public InventorySlot GetEmptyWisdomPowerupSlot()
        {
            return GetEmptySlotOfType(SlotType.WisdomPowerupSlot);
        }

        public InventorySlot GetEmptyResurrectionSlot()
        {
            return GetEmptySlotOfType(SlotType.ResurrectionSlot);
        }


    }
}
