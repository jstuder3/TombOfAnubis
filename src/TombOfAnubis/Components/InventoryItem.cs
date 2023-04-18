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
                    ItemType = ItemType.None;

                    ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
                    pec.LocalPosition = new Vector2(25f, 25f);
                    pec.RandomizedSpawnPositionRadius = 20f;
                    pec.Texture = ParticleTextureLibrary.FourCornerStar;
                    pec.RandomizedTintMin = Color.LightBlue;
                    pec.RandomizedTintMax = Color.DarkBlue;
                    pec.Scale = Vector2.One * 0.4f;
                    pec.ScalingMode = ScalingMode.LinearDecreaseToZero;
                    pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
                    pec.EmitterDuration = 5f;
                    pec.ParticleDuration = 1f;
                    pec.EmissionFrequency = 60f;
                    pec.EmissionRate = 1f;
                    pec.InitialSpeed = 100f;
                    pec.SpawnDirection = new Vector2(0f, -1f);
                    pec.SpawnConeDegrees = 360f;
                    pec.Drag = 0.5f;

                    Entity.AddComponent(new ParticleEmitter(pec));

                    Console.WriteLine("Speedup applied!");
                    return true;
                case ItemType.Fist:
                    Session singleton = Session.GetInstance();
                    Vector2 forwardVector = Entity.GetComponent<Movement>().GetForwardVector();
                    Fist fist = new Fist(Entity.GetComponent<Transform>().Position, forwardVector);
                    Session.GetInstance().Scene.AddChild(fist);
                    //make the fist move automatically and make it despawn automatically
                    fist.AddComponent(new GameplayEffect(EffectType.LinearAutoMove, 0.3f, 1000f, forwardVector));
                    fist.AddComponent(new GameplayEffect(EffectType.Lifetime, 0.3f));
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
