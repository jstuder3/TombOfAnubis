using System;

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

    public class InventoryItem : Component
    {
        public ItemType ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool isInWorld = false;
        public bool isInInventory = false;

        public InventoryItem(ItemType itemType, Entity entity)
        {
            ItemType = itemType;
            Entity = entity;
        }

        public bool TryUse()
        {
            switch (ItemType)
            {
                case ItemType.None:
                    return false;
                case ItemType.Speedup:
                    Entity.AddComponent(new GameplayEffect(EffectType.Speedup, 10f));
                    ItemType = ItemType.None;
                    Console.WriteLine("Speedup applied!");
                    return true;
            }
            return false;
        }

    }
}
