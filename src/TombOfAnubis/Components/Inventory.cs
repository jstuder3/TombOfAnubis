using System.Collections.Generic;

namespace TombOfAnubis
{
    public class Inventory : Component
    {
        //public List<InventorySlot> InventorySlots { get; set; }
        
        public List<InventorySlot> ArtefactSlots { get; set; }
        public List<InventorySlot> BodySlots { get; set; }
        public List<InventorySlot> WisdomSlots { get; set; }
        public List<InventorySlot> ResurrectionSlots { get; set; }

        public Inventory(int numArtefactSlots, int numItemSlots)
        {
            //InventorySlots = new List<InventorySlot>();
            ArtefactSlots = new List<InventorySlot>();
            BodySlots = new List<InventorySlot>();
            WisdomSlots = new List<InventorySlot>();
            ResurrectionSlots = new List<InventorySlot>();

            for (int i = 0; i < numArtefactSlots; i++)
            {
                ArtefactSlots.Add(new InventorySlot(i, SlotType.ArtefactSlot));
            }

            //for now we only have one slot for each powerup type
            for(int i = 0; i < 1; i++)
            {
                BodySlots.Add(new InventorySlot(i, SlotType.BodyPowerupSlot));
                WisdomSlots.Add(new InventorySlot(i, SlotType.WisdomPowerupSlot));
                ResurrectionSlots.Add(new InventorySlot(i, SlotType.ResurrectionSlot));
            }

            //with the current version, we always have one item slot per SlotType
            /*for (int i = numArtefactSlots; i < numArtefactSlots + numItemSlots; i++)
            {
                InventorySlots.Add(new InventorySlot(i, SlotType.ItemSlot));
            }*/

            //InventorySlots.Add(new InventorySlot(numArtefactSlots, SlotType.BodyPowerupSlot));
            //InventorySlots.Add(new InventorySlot(numArtefactSlots + 1, SlotType.WisdomPowerupSlot));
            //InventorySlots.Add(new InventorySlot(numArtefactSlots + 2, SlotType.ResurrectionSlot));

        }

        public void AddArtefact(int slotIndex = 0)
        {
            if (slotIndex < ArtefactSlots.Count)
            {
                InventorySlot slot = ArtefactSlots[slotIndex];
                if (slot.IsEmpty() && slot.SlotType == SlotType.ArtefactSlot)
                {
                    slot.Item = new InventoryItem(ItemType.Artefact);
                }
            }
        }

        public override void Delete()
        {
            base.Delete();
            //InventorySlots = null;
            BodySlots = null;
            WisdomSlots = null;
            ResurrectionSlots = null;
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
            else if(slotType == SlotType.BodyPowerupSlot)
            {
                foreach (InventorySlot slot in BodySlots)
                {
                    if (slot.SlotType == slotType && slot.IsEmpty()) return slot;
                }
            }
            
            else if (slotType == SlotType.WisdomPowerupSlot)
            {
                foreach (InventorySlot slot in WisdomSlots)
                {
                    if (slot.SlotType == slotType && slot.IsEmpty()) return slot;
                }
            }
            else if (slotType == SlotType.ResurrectionSlot)
            {
                foreach (InventorySlot slot in ResurrectionSlots)
                {
                    if (slot.SlotType == slotType && slot.IsEmpty()) return slot;
                }
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

        public bool HasResurrectItem()
        {
            return GetFullResurrectionSlot() != null;
        }
        public InventorySlot GetFullResurrectionSlot()
        {
            foreach(InventorySlot slot in ResurrectionSlots)
            {
                if (!slot.IsEmpty()) return slot;
            }
            return null;
        }

    }
}
