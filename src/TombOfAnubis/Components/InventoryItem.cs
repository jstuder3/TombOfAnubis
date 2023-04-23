using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Vector2 = Microsoft.Xna.Framework.Vector2;

using Microsoft.Xna.Framework.Content;

namespace TombOfAnubis
{
    public struct ItemTextureLibrary
    {
        public static Texture2D Speedup { get; set; }
        public static Texture2D Resurrection { get; set; }
        public static Texture2D Fist { get; set; }
        //public static Texture2D Artefact { get; set; }

        public static Texture2D GetTexture(ItemType itemType)
        {
            switch(itemType)
            {
                case ItemType.Speedup: return Speedup;
                case ItemType.Resurrection: return Resurrection;
                case ItemType.Fist: return Fist;
                //case ItemType.Artefact: return Artefact;
            }
            return null;
        }

        public static void LoadContent(GameScreenManager gameScreenManager)
        {
            ContentManager content = gameScreenManager.Game.Content;

            // load textures into static variables of ParticleTextureLibrary, so they can be used as a texture library
            string item_base_path = "Textures/Objects/Items/PowerUp/";
            ItemTextureLibrary.Speedup = content.Load<Texture2D>(item_base_path + "SpeedUp");
            ItemTextureLibrary.Resurrection = content.Load<Texture2D>(item_base_path + "Resurrection");
            ItemTextureLibrary.Fist = content.Load<Texture2D>(item_base_path + "Fist");
            //ItemTextureLibrary.Artefact = ???
        }
    }

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
                    Entity.AddComponent(new GameplayEffect(EffectType.AdditiveSpeedModification, 5f, 200f, Visibility.Game));
                    ItemType = ItemType.None;

                    ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
                    pec.LocalPosition = new Vector2(25f, 25f);
                    pec.RandomizedSpawnPositionRadius = 20f;
                    pec.Texture = ParticleTextureLibrary.BasicParticle;
                    pec.SpriteLayer = 1;
                    pec.RandomizedTintMin = Color.Red;
                    pec.RandomizedTintMax = Color.Orange;
                    pec.Scale = Vector2.One * 0.4f;
                    pec.InitialAlpha = 1f;
                    pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
                    pec.ScalingMode = ScalingMode.Constant;
                    pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
                    pec.EmitterDuration = 5f;
                    pec.ParticleDuration = 1f;
                    pec.EmissionFrequency = 60f;
                    pec.EmissionRate = 1f;
                    pec.InitialSpeed = 200f;
                    pec.SpawnDirection = new Vector2(0f, -1f);
                    pec.SpawnConeDegrees = 360f;
                    pec.Drag = 0.5f;

                    Entity.AddComponent(new ParticleEmitter(pec));

                    Console.WriteLine("Speedup applied!");
                    return true;
                case ItemType.Fist:
                    Session singleton = Session.GetInstance();
                    Vector2 forwardVector = Entity.GetComponent<Movement>().GetForwardVector();
                    Fist fist = new Fist(Entity.TopLeftCornerPosition(), forwardVector);
                    //fist.Position() == Entity.Position();
                    Session.GetInstance().World.AddChild(fist);
                    //make the fist move automatically and make it despawn automatically
                    fist.AddComponent(new GameplayEffect(EffectType.LinearAutoMove, 0.3f, 1000f, forwardVector, Visibility.Game));
                    fist.AddComponent(new GameplayEffect(EffectType.Lifetime, 0.3f, Visibility.Game));
                    ItemType = ItemType.None;
                    Console.WriteLine("Fist spawned!");
                    return true;
                case ItemType.HidingCloak:
                    Entity.AddComponent(new GameplayEffect(EffectType.Hidden, 5f, Visibility.Game));
                    Entity.AddComponent(new GameplayEffect(EffectType.MultiplicativeSpeedModification, 5f, 0.5f, Visibility.Game));
                    ItemType = ItemType.None;
                    Console.WriteLine("Used HidingCloak!");
                    break;
            }
            return false;
        }

        public void DropItem()
        {
            if(ItemType == ItemType.None) return;
            Transform transform = Entity.GetComponent<Transform>();
            Movement movement = Entity.GetComponent<Movement>();

            Vector2 forwardVector = movement.GetForwardVector();

            Session.GetInstance().World.AddChild(new WorldItem(transform.Position + forwardVector * 100f, transform.Scale, ItemType));
            ItemType = ItemType.None;
            Console.WriteLine("Dropped item!");

        }

        public Texture2D GetTexture()
        {
            return ItemTextureLibrary.GetTexture(ItemType);
        }

    }
}
