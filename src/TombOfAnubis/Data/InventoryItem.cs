namespace TombOfAnubis
{
    public enum ItemType
    {
        None,
        Speedup,
        IncreaseViewDistance,
        Resurrection,
        Artefact
    }

    public class InventoryItem
    {
        public ItemType ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool isInWorld = false;
        public bool isInInventory = false;
        public InventoryItem()
        {
            ItemType = ItemType.None;
        }
        public InventoryItem(ItemType itemType)
        {
            ItemType = itemType;
        }

    }
}
