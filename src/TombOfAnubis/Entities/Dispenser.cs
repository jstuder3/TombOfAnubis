using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public enum DispenserType
    {
        ItemDispenser,
        /*BodyPowerup,
        WisdomPowerup,
        ResurrectionPowerup,*/
        None
    }
    public class Dispenser : Entity, ICooldown
    {
        DispenserType dispenserType = DispenserType.None;
        private bool isOnCooldown = false;

        public float CooldownDuration { get; set; }

        private ParticleEmitter particleEmitter { get; set; }

        public ItemType ItemType { get; set; }
        //public Sprite ItemSprite { get; set; }
        private ParticleEmitter itemEmitter { get; set; }

        private Random random;

        public Dispenser(Vector2 position, Vector2 scale, Texture2D texture, List<AnimationClip> animationClips, DispenserType dispenserType)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(position, 4f * scale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Sprite sprite;
            if (animationClips != null)
            {
                Animation animation = new Animation(animationClips, Visibility.Both);
                AddComponent(animation);

                animation.SetActiveClip(AnimationClipType.Open);

                sprite = new Sprite(texture, animation.DefaultSourceRectangle, 2, Visibility.Both);
            }
            else
            {
                sprite = new Sprite(texture, 2, Visibility.Both);
            }
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), true);
            AddComponent(collider);

            Discovery discovery = new Discovery();
            AddComponent(discovery);

            this.dispenserType = dispenserType;

            // set up cooldown
            this.CooldownDuration = 5f;

            this.random = new Random();

            EndCooldown();

            Initialize();
        }

        public bool TryGiveItem(Inventory inventory, double currentTime)
        {
            //only give an item if the dispenser isn't on cooldown
            if (IsOnCooldown()) return false;

            InventorySlot emptyItemSlot;

            //if there is space, put an item in the empty slot according to which type of dispenser this is

            emptyItemSlot = inventory.GetEmptyItemSlot();
            if (emptyItemSlot == null)
            {
                Console.WriteLine("No more space in inventory!");
                return false;
            }

            emptyItemSlot.Item = new InventoryItem(ItemType, emptyItemSlot.Entity);

            ItemType = ItemType.None;
            itemEmitter.EndEmitter();

            // put dispenser on cooldown
            AddComponent(new GameplayEffect(EffectType.OnCooldown, CooldownDuration, Visibility.Both));

            return true;

        }

        public bool IsOnCooldown()
        {
            return isOnCooldown;
        }

        public void PutOnCooldown()
        {
            isOnCooldown = true;

            GetComponent<Animation>().SetActiveClip(AnimationClipType.Closed);
            particleEmitter.EndEmitter();

            itemEmitter.EndEmitter();

        }

        public void EndCooldown()
        {
            isOnCooldown = false;
            GetComponent<Animation>().SetActiveClip(AnimationClipType.Open);

            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(65f, 30f);
            pec.RandomizedSpawnPositionRadius = 40f;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 4;
            pec.RandomizedTintMin = Color.Yellow;
            pec.RandomizedTintMax = Color.LightYellow;
            pec.Scale = Vector2.One * 0.2f;
            pec.ScalingMode = ScalingMode.LinearDecreaseToZero;
            pec.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
            pec.EmitterDuration = 0f;
            pec.ParticleDuration = 2f;
            pec.EmissionFrequency = 5f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 10f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 360f;
            pec.Drag = 0.5f;

            particleEmitter = new ParticleEmitter(pec);

            AddComponent(particleEmitter);

            Texture2D itemTexture = ItemTextureLibrary.Speedup;

            if (dispenserType == DispenserType.ItemDispenser)
            {

                switch (random.Next(0, 3))
                {
                    case 0: //Speedup
                        ItemType = ItemType.Speedup;
                        itemTexture = ItemTextureLibrary.Speedup;
                        break;
                    case 1: //Fist
                        ItemType = ItemType.Fist;
                        itemTexture = ItemTextureLibrary.Fist;
                        break;
                    case 2: //Resurrection (is now a self-revive item)
                        ItemType = ItemType.Resurrection;
                        itemTexture = ItemTextureLibrary.Resurrection;
                        break;
                    case 3: //Hiding Cloak (not yet implemented)
                        ItemType = ItemType.HidingCloak;
                        //ItemSprite = new Sprite(ItemTextureLibrary.HidingCloak, 3, Visibility.Game);
                        break;
                    default:
                        return;
                }
            }
            else
            {
                Console.WriteLine("Unknown dispenser type!");
                return;
            }

            ParticleEmitterConfiguration item_pec = new ParticleEmitterConfiguration();
            item_pec.LocalPosition = new Vector2(50f, 10f);
            item_pec.RandomizedSpawnPositionRadius = 0f;
            item_pec.Texture = itemTexture;
            item_pec.SpriteLayer = 3;
            item_pec.RandomizedTintMin = Color.White;
            item_pec.RandomizedTintMax = Color.White;
            item_pec.Scale = Vector2.One * 0.3f;
            item_pec.EmitterDuration = 0f;
            item_pec.ParticleDuration = 0.06f;
            item_pec.EmissionFrequency = 20f;
            item_pec.EmissionRate = 1f;

            itemEmitter = new ParticleEmitter(item_pec);
            AddComponent(itemEmitter);



        }

    }
}
