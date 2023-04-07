using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TombOfAnubis
{
    public enum ItemType
    {
        None,
        Speedup,
        IncreaseViewDistance,
        Resurrection,
        Fist,
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
                    Entity.AddComponent(new GameplayEffect(EffectType.Speedup, 10f, 400f));
                    ItemType = ItemType.None;
                    Console.WriteLine("Speedup applied!");
                    return true;
                case ItemType.Fist:
                    Session singleton = Session.GetInstance();
                    Fist fist = new Fist(Entity.GetComponent<Transform>().Position, singleton.Map.Fist.Scale, singleton.Map.Fist.Texture, null);
                    Session.GetInstance().Scene.AddChild(fist);
                    //make the fist move automatically and make it despawn automatically
                    fist.AddComponent(new GameplayEffect(EffectType.AutoMove, 0.2f, 400f, new Vector2(1f, 0f)));
                    fist.AddComponent(new GameplayEffect(EffectType.Lifetime, 0.2f));
                    ItemType = ItemType.None;
                    Console.WriteLine("Fist spawned!");
                    return true;
            }
            return false;
        }

    }
}
