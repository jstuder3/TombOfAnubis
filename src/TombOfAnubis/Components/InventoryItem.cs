using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public enum ItemType
    {
        None,
        Speedup,
        IncreaseViewDistance,
        Resurrection,
        Fist,
        HidingCloak,
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
                    Entity.AddComponent(new GameplayEffect(EffectType.AdditiveSpeedModification, 5f, 200f));
                    //Entity.AddComponent(new GameplayEffect(EffectType.MultiplicativeSpeedModification, 5f, 1.5f));
                    ItemType = ItemType.None;
                    Console.WriteLine("Speedup applied!");
                    return true;
                case ItemType.Fist:
                    Session singleton = Session.GetInstance();
                    Vector2 forwardVector = Entity.GetComponent<Movement>().GetForwardVector();
                    Fist fist = new Fist(Entity.GetComponent<Transform>().Position, singleton.Map.Fist.Scale, singleton.Map.Fist.Texture, singleton.Map.Fist.Animation, forwardVector);
                    Session.GetInstance().Scene.AddChild(fist);
                    //make the fist move automatically and make it despawn automatically
                    fist.AddComponent(new GameplayEffect(EffectType.LinearAutoMove, 1f, 600f, forwardVector));
                    fist.AddComponent(new GameplayEffect(EffectType.Lifetime, 1f));
                    ItemType = ItemType.None;
                    Console.WriteLine("Fist spawned!");
                    return true;
                case ItemType.HidingCloak:
                    Entity.AddComponent(new GameplayEffect(EffectType.Hidden, 5f));
                    Entity.AddComponent(new GameplayEffect(EffectType.MultiplicativeSpeedModification, 5f, 0.5f));
                    ItemType = ItemType.None;
                    Console.WriteLine("Used HidingCloak!");
                    break;
            }
            return false;
        }

    }
}
