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
        public static Texture2D HidingCloak { get; set; }
        public static Texture2D AnubisLocationReveal { get; set; }
        public static Texture2D Teleport { get; set; }

        //public static Texture2D Artefact { get; set; }

        public static Texture2D GetTexture(ItemType itemType)
        {
            switch(itemType)
            {
                case ItemType.Speedup: return Speedup;
                case ItemType.Resurrection: return Resurrection;
                case ItemType.Fist: return Fist;
                case ItemType.HidingCloak: return HidingCloak;
                case ItemType.AnubisLocationReveal: return AnubisLocationReveal;
                case ItemType.Teleport: return Teleport;
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
            ItemTextureLibrary.HidingCloak = content.Load<Texture2D>(item_base_path + "HidingCloak");
            ItemTextureLibrary.AnubisLocationReveal = content.Load<Texture2D>(item_base_path + "AnubisLocationReveal");
            ItemTextureLibrary.Teleport = content.Load<Texture2D>(item_base_path + "Teleport");
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
        AnubisLocationReveal,
        Artefact,
        Teleport
    }

    public class InventoryItem : Component
    {
        public ItemType ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool isInWorld = false;
        public bool isInInventory = false;

        private float dropCooldownEnd = 0f;

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
                case ItemType.Resurrection:
                    if(Entity.GetComponent<Movement>().State == MovementState.Trapped)
                    {

                        if (((Character)Entity).Ghost != null)
                        {
                            ((Character)Entity).Ghost.Delete();
                            ((Character)Entity).Ghost = null;
                        }

                        ItemType = ItemType.None;

                        Entity.GetComponent<Movement>().State = MovementState.Idle;
                        Entity.GetComponent<Animation>()?.SetActiveClip(AnimationClipType.WalkingDown);

                        ParticleEmitterConfiguration pec2 = new ParticleEmitterConfiguration();
                        pec2.LocalPosition = new Vector2(30f, 30f);
                        pec2.RandomizedSpawnPositionRadius = 40f;
                        pec2.Texture = ParticleTextureLibrary.PlusFilledWithOutline;
                        pec2.SpriteLayer = 3;
                        pec2.RandomizedTintMin = Color.Chartreuse;
                        pec2.RandomizedTintMax = Color.Green;
                        pec2.Scale = Vector2.One * 0.4f;
                        pec2.ScalingMode = ScalingMode.LinearDecreaseToZero;
                        pec2.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
                        pec2.EmitterDuration = 1f;
                        pec2.ParticleDuration = 1f;
                        pec2.EmissionFrequency = 30f;
                        pec2.EmissionRate = 1f;
                        pec2.InitialSpeed = 40f;
                        pec2.SpawnDirection = new Vector2(0f, -1f);
                        pec2.SpawnConeDegrees = 360f;
                        pec2.Drag = 0.5f;

                        Entity.AddComponent(new ParticleEmitter(pec2));

                    }

                    break;
                case ItemType.HidingCloak:
                    foreach(GameplayEffect ge in Entity.GetComponentsOfType<GameplayEffect>()) //cannot use another hiding cloak if already hidden. this prevents some nasty bugs
                    {
                        if(ge.Type == EffectType.Hidden && !ge.HasEnded())
                        {
                            return false;
                        }
                    }
                    Entity.AddComponent(new GameplayEffect(EffectType.Hidden, 5f, Visibility.Game));
                    Entity.AddComponent(new GameplayEffect(EffectType.MultiplicativeSpeedModification, 5f, 0.5f, Visibility.Game));

                    Texture2D cloakTexture = ItemTextureLibrary.HidingCloak;
                    float cloakScale = 1.5f;
                    Vector2 position = new Vector2(Entity.GetComponent<Sprite>().SourceRectangle.Width / 2, Entity.GetComponent<Sprite>().SourceRectangle.Height / 2) - new Vector2(cloakTexture.Width / 2, cloakTexture.Height / 2) * cloakScale;
                    HidingCloak hidingCloak = new HidingCloak(position, Vector2.One*cloakScale, cloakTexture);
                    Entity.AddChild(hidingCloak);
                    hidingCloak.AddComponent(new GameplayEffect(EffectType.Lifetime, 5f, Visibility.Both));


                    ItemType = ItemType.None;
                    Console.WriteLine("Used HidingCloak!");
                    break;
                case ItemType.AnubisLocationReveal:
                    //Entity.AddComponent(new GameplayEffect(EffectType.Hidden, 5f, Visibility.Game));
                    //Entity.AddComponent(new GameplayEffect(EffectType.MultiplicativeSpeedModification, 5f, 0.5f, Visibility.Game));

                    
                    foreach(Anubis anubis in Session.GetInstance().World.GetChildrenOfType<Anubis>())
                    {
                        AnubisLocator anubisLocator = new AnubisLocator(anubis.GetComponent<Transform>().Position, Vector2.One * 5f);
                        anubisLocator.AddComponent(new GameplayEffect(EffectType.Lifetime, 5f, Visibility.Both));
                        anubisLocator.AddComponent(new GameplayEffect(EffectType.DelayedFollow, 5f, 0.1f, anubis, Visibility.Both));
                    }
                    

                    ItemType = ItemType.None;
                    Console.WriteLine("Used HidingCloak!");
                    break;
                case ItemType.Teleport:
                    //Entity.AddComponent(new GameplayEffect(EffectType.LinearAutoMove, 5f, 100f, new Vector2(1f, -1f), Visibility.Both));
                    //return true;
                    Vector2 playerCenterPosition = Entity.CenterPosition();
                    Vector2 forwardDirection = Entity.GetComponent<Movement>().GetForwardVector();
                    forwardDirection.Normalize();
                    Vector2 tileLength = Session.GetInstance().Map.TileSize;
                    Vector2 teleTranslation = new Vector2(2 * tileLength.X * forwardDirection.X, 2 * tileLength.Y * forwardDirection.Y);

                    //check if targetposition is valid for all 4 corners of the player
                    Vector2 HalfDiagPlayerTranslation = Entity.CenterPosition() - Entity.TopLeftCornerPosition();
                    Vector2 CenterToTopRightTranslation = new Vector2(HalfDiagPlayerTranslation.X, -HalfDiagPlayerTranslation.Y);

                    bool topLeft = Session.GetInstance().Map.GetCollisionLayerValue(Session.GetInstance().Map.PositionToTileCoordinate(Entity.TopLeftCornerPosition() + teleTranslation)) == 0;
                    bool bottomRight = Session.GetInstance().Map.GetCollisionLayerValue(Session.GetInstance().Map.PositionToTileCoordinate(Entity.TopLeftCornerPosition() + 2 * HalfDiagPlayerTranslation + teleTranslation)) == 0;
                    bool topRight = Session.GetInstance().Map.GetCollisionLayerValue(Session.GetInstance().Map.PositionToTileCoordinate(Entity.CenterPosition() + CenterToTopRightTranslation + teleTranslation)) == 0;
                    bool bottomLeft = Session.GetInstance().Map.GetCollisionLayerValue(Session.GetInstance().Map.PositionToTileCoordinate(Entity.CenterPosition() - CenterToTopRightTranslation + teleTranslation)) == 0;

                    Console.WriteLine("curTopLeftPosition: " + Entity.TopLeftCornerPosition());
                    Console.WriteLine("targetTopLeftPosition: " + (Entity.TopLeftCornerPosition() + teleTranslation));

                    if (topLeft && topRight && bottomRight && bottomLeft)
                    {

                        int num_streak_spawners = 10;

                        //paticles that are spawned at the old location and move to the new location to indicate the teleport
                        ParticleEmitterConfiguration teleport_streak = new ParticleEmitterConfiguration();
                        
                        teleport_streak.RandomizedSpawnPositionRadius = 50f;
                        teleport_streak.Texture = ParticleTextureLibrary.BasicParticle;
                        teleport_streak.SpriteLayer = 1;
                        teleport_streak.RandomizedTintMin = Color.Yellow;
                        teleport_streak.RandomizedTintMax = Color.White;
                        teleport_streak.Scale = Vector2.One * 0.2f;
                        teleport_streak.ScalingMode = ScalingMode.Constant;
                        teleport_streak.InitialAlpha = 1f;
                        teleport_streak.AlphaMode = AlphaMode.LinearDecreaseToZero;
                        teleport_streak.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
                        teleport_streak.EmitterDuration = 0.1f;
                        teleport_streak.ParticleDuration = 1f;
                        teleport_streak.EmissionFrequency = 30f;
                        teleport_streak.EmissionRate = 5f;
                        teleport_streak.InitialSpeed = 150f;
                        teleport_streak.SpawnDirection = forwardDirection;//new Vector2(0f, -1f);
                        teleport_streak.SpawnConeDegrees = 160f;
                        teleport_streak.Drag = 0.5f;
                        //teleport_streak.LocalPointForcePosition = playerCenterPosition + teleTranslation;
                        //teleport_streak.PointForceStrength = 500f;

                        for(int i = 0;  i< num_streak_spawners; i++)
                        {
                            teleport_streak.LocalPosition = playerCenterPosition + (float)i / (float)num_streak_spawners * teleTranslation;
                            Session.GetInstance().World.AddComponent(new ParticleEmitter(teleport_streak));
                        }

                        

                        //use teleport
                        Entity.GetComponent<Transform>().Position = Entity.TopLeftCornerPosition() + teleTranslation;

                        //particles that are spawned at the new location to show an "impact"
                        ParticleEmitterConfiguration teleport_impact = new ParticleEmitterConfiguration();
                        teleport_impact.LocalPosition = Entity.GetComponent<Transform>().Position;
                        teleport_impact.RandomizedSpawnPositionRadius = 50f;
                        teleport_impact.Texture = ParticleTextureLibrary.BasicParticle;
                        teleport_impact.SpriteLayer = 1;
                        teleport_impact.RandomizedTintMin = Color.DarkGray;
                        teleport_impact.RandomizedTintMax = Color.Gray;
                        teleport_impact.Scale = Vector2.One * 0.6f;
                        teleport_impact.ScalingMode = ScalingMode.Constant;
                        teleport_impact.InitialAlpha = 1f;
                        teleport_impact.AlphaMode = AlphaMode.LinearDecreaseToZero;
                        teleport_impact.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
                        teleport_impact.EmitterDuration = 0.10f;
                        teleport_impact.ParticleDuration = 2f;
                        teleport_impact.EmissionFrequency = 20f;
                        teleport_impact.EmissionRate = 50f;
                        teleport_impact.InitialSpeed = 150f;
                        teleport_impact.SpawnDirection = new Vector2(0f, -1f);
                        teleport_impact.SpawnConeDegrees = 360f;
                        teleport_impact.Drag = 0.5f;

                        Session.GetInstance().World.AddComponent(new ParticleEmitter(teleport_impact));

                        Console.WriteLine("Used Teleport, new location: " + Entity.TopLeftCornerPosition());
                        ItemType = ItemType.None;
                        return true;
                    }
                    //can't use teleport
                    Console.WriteLine("Teleport: There's a time and place for everything, but not now.");
                    return false;
            }
            return false;
        }

        public void DropItem(GameTime gameTime)
        {
            if(ItemType == ItemType.None) return;
            if ((float)gameTime.TotalGameTime.TotalSeconds < dropCooldownEnd) return;

            //if ((float)gameTime.TotalGameTime.TotalSeconds - lastTimeDropped > 0.5f)
            {

                Transform transform = Entity.GetComponent<Transform>();
                Movement movement = Entity.GetComponent<Movement>();

                Vector2 forwardVector = movement.GetForwardVector();
                WorldItem wi = new WorldItem(transform.Position + forwardVector * 150f, transform.Scale, ItemType);
                Session.GetInstance().World.AddChild(wi);
                wi.AddComponent(new GameplayEffect(EffectType.Lifetime, 5f, Visibility.Both));

                //put dropping on cooldown
                dropCooldownEnd = (float)gameTime.TotalGameTime.TotalSeconds + 1f;

                ItemType = ItemType.None;
                Console.WriteLine("Dropped item!");
            }
            return;
        }

        public Texture2D GetTexture()
        {
            return ItemTextureLibrary.GetTexture(ItemType);
        }

    }
}
