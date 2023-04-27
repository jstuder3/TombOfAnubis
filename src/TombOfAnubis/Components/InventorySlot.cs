using Microsoft.Xna.Framework;

namespace TombOfAnubis
{
    public enum SlotType
    {
        ArtefactSlot,
        ItemSlot
    }
    public class InventorySlot : Component
    {

        public InventoryItem Item { get; set; }
        public int SlotNumber { get; set; } = -1;
        public SlotType SlotType { get; set; }
        public InventorySlot(int slotNumber, SlotType slotType, Entity entity)
        {
            Item = new InventoryItem(ItemType.None, Entity);
            SlotNumber = slotNumber;
            SlotType = slotType;
            Entity = entity;

        }
        public void ClearItem()
        {
            Item.ItemType = ItemType.None;
        }

        public void SetItem(ItemType itemType)
        {
            Item.ItemType = itemType;
        }

        public bool IsEmpty()
        {
            return Item == null || Item.ItemType == ItemType.None;
        }

        public bool TryUseItem()
        {
            return Item.TryUse();
        }

        public void DropItem(GameTime gameTime)
        {
            Item.DropItem(gameTime);
        }

    }
}
