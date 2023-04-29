using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using static TombOfAnubis.Character;

namespace TombOfAnubis
{
    public static class EntityGenerator
    {
        private static ContentManager content;
        private static List<Entity> entities;
        public static void Initialize(ContentManager _content)
        {
            content = _content;
        }
        public static List<Entity> GenerateEntities(List<EntityDescription> EntityDescriptions) 
        {
            Session.GetInstance().Map.Artefacts = new List<EntityDescription>();

            entities = new List<Entity>();

            foreach (EntityDescription entityDescription in EntityDescriptions)
            {
                Type t = Type.GetType(entityDescription.ClassName);
                if(t == typeof(Character))
                {
                    Enum.TryParse(entityDescription.Type, out CharacterType type);
                    if ((int)type < Session.GetInstance().NumberOfPlayers)
                    {
                        entities.Add(SpawnCharacter(entityDescription));
                    }
                }
                else if(t == typeof(Artefact)) {
                    entities.Add(SpawnArtefact(entityDescription));
                }
                else if (t == typeof(Dispenser)) {
                    entities.Add(SpawnDispenser(entityDescription));
                }
                else if (t == typeof(Anubis)) {
                    entities.Add(SpawnAnubis(entityDescription));
                }
                else if (t == typeof(Altar)) {
                    entities.Add(SpawnAltar(entityDescription));
                }
                else if (t == typeof(Trap)) {
                    entities.Add(SpawnTrap(entityDescription));
                }
                else if (t == typeof(Button)) {
                    entities.Add(SpawnButton(entityDescription));
                }
            }
            return entities;
        }
        public static Entity SpawnCharacter(EntityDescription entityDescription)
        {
            Enum.TryParse(entityDescription.Type, out CharacterType type);
            entityDescription.SpriteTextureName = Session.GetInstance().CharacterTextures[(int)type].Name;
            entityDescription.GhostSpriteTextureName = Session.GetInstance().GhostCharacterTextures[(int)type].Name;

            entityDescription.Load(content, "");
            return new Character( entityDescription);
        }
        public static Entity SpawnArtefact(EntityDescription entityDescription)
        {   
            Session.GetInstance().Map.Artefacts.Add(entityDescription);
            Enum.TryParse(entityDescription.Type, out CharacterType type);
            entityDescription.SpriteTextureName = Session.GetInstance().ArtefactTextures[(int)type].Name;
            entityDescription.Load(content, "");

            return new Artefact(
                    (int)type,
                    Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                    entityDescription.Scale,
                    entityDescription.Scale * 6,
                    entityDescription.Texture,
                    true
                    );
        }
        public static Entity SpawnDispenser(EntityDescription entityDescription)
        {
            entityDescription.Load(content, @"Textures\Objects\Dispensers");

            _ = Enum.TryParse(entityDescription.Type, out DispenserType type);
            return new Dispenser(
                Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                entityDescription.Scale,
                entityDescription.Texture,
                entityDescription.Animation,
                type
                );
        }
        public static Entity SpawnAnubis(EntityDescription entityDescription)
        {
            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(25f, 100f);
            pec.RandomizedSpawnPositionRadius = 50f;
            //doesn't work yet
            pec.ParticlesMoveWithEntity = false;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 1;
            pec.InitialAlpha = 0.5f;
            pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
            pec.RandomizedTintMin = Color.SlateGray;
            pec.RandomizedTintMax = Color.DimGray;
            pec.Scale = Vector2.One * 0.4f;
            pec.ScalingMode = ScalingMode.Constant;
            pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
            pec.EmitterDuration = 0f;
            pec.ParticleDuration = 2f;
            pec.EmissionFrequency = 60f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 50f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 90f;
            pec.Gravity = new Vector2(0f, 0f);
            //currently behaves a bit unintuitively
            pec.LocalPointForcePosition = Vector2.Zero;
            pec.PointForceStrength = 0f;
            pec.PointForceUsesQuadraticFalloff = false;
            pec.Gravity = new Vector2(0f, 0f);
            pec.Drag = 0.5f;
            pec.Visibility = Visibility.Game;

            entityDescription.Load(content, @"Textures\Characters");
            Anubis anubis = new Anubis(
                        Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                        entityDescription.Scale,
                        entityDescription.Texture,
                        entityDescription.Animation,
                        Session.GetInstance().Map.EntityProperties.MaxAnubisMovementSpeed,
                        Session.GetInstance().Map);
            anubis.AddComponent(new ParticleEmitter(pec));
            return anubis;
        }
        public static Entity SpawnAltar(EntityDescription entityDescription)
        {
            entityDescription.Load(content, @"Textures\Objects\Altar");
            return new Altar(
                        Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                        entityDescription.Scale,
                        entityDescription.Texture,
                        Session.GetInstance().NumberOfPlayers);
        }
        public static Entity SpawnTrap(EntityDescription entityDescription)
        {
            entityDescription.Load(content, @"Textures\Objects\Traps");
            _ = Enum.TryParse(entityDescription.Type, out TrapType type);
            return new Trap(
                type,
                Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                entityDescription.Scale,
                entityDescription.Texture,
                entityDescription.Animation
                );
        }
        public static Entity SpawnButton(EntityDescription entityDescription)
        {
            entityDescription.Load(content, @"Textures\Objects\Buttons");

            List<Vector2> connectedTraps = new List<Vector2>();

            foreach (EntityDescription trapEntity in entityDescription.ConnectedTrapPositions)
            {
                connectedTraps.Add(Session.GetInstance().Map.CreateEntityTileCenteredPositionSpriteless(trapEntity));
            }
            _ = Enum.TryParse(entityDescription.Type, out ButtonType type);
            return new Button(
                type,
                Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                entityDescription.Scale,
                entityDescription.Texture,
                entityDescription.Animation,
                connectedTraps,
                entities
                );
        }
    }
}
