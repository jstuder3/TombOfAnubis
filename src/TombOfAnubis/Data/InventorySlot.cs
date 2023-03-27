namespace TombOfAnubis
{
    public enum SlotType
    {
        ArtefactSlot,
        ItemSlot
    }
    public class InventorySlot
    {

        public InventoryItem Item { get; set; }
        public int SlotNumber { get; set; } = -1;
        public SlotType SlotType { get; set; }
        public InventorySlot(int slotNumber, SlotType slotType)
        {
            SlotNumber = slotNumber;
            SlotType = slotType;
        }
        public void ClearItem()
        {
            Item = null;
        }
        public bool IsEmpty()
        {
            return Item == null;
        }
    }
}
