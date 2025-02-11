﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System; using System.Diagnostics;
using System.Collections.Generic;
using static TombOfAnubis.Character;

namespace TombOfAnubis
{
    public static class EntityGenerator
    {
        private static ContentManager content;
        private static List<Entity> entities;
        public static List<Type> DoNotSpawnTypes { get; set; } = new List<Type>();
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
                if(t == typeof(Character) && !DoNotSpawnTypes.Contains(t))
                {
                    Enum.TryParse(entityDescription.Type, out CharacterType type);
                    if ((int)type < Session.GetInstance().NumberOfPlayers)
                    {
                        entities.Add(SpawnCharacter(entityDescription));
                    }
                }
                else if(t == typeof(Artefact) && !DoNotSpawnTypes.Contains(t)) {
                    entities.Add(SpawnArtefact(entityDescription));
                }
                else if (t == typeof(Dispenser) && !DoNotSpawnTypes.Contains(t)) {
                    entities.Add(SpawnDispenser(entityDescription));
                }
                else if (t == typeof(Anubis) && !DoNotSpawnTypes.Contains(t)) {
                    entities.Add(SpawnAnubis(entityDescription));
                }
                else if (t == typeof(Altar) && !DoNotSpawnTypes.Contains(t)) {
                    entities.Add(SpawnAltar(entityDescription));
                }
                else if (t == typeof(Trap) && !DoNotSpawnTypes.Contains(t)) {
                    entities.Add(SpawnTrap(entityDescription));
                }
                else if (t == typeof(Button) && !DoNotSpawnTypes.Contains(t)) {
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

            entityDescription.Load(content, @"Textures\Characters");
            Anubis anubis = new Anubis(
                        Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                        entityDescription.Scale,
                        entityDescription.Texture,
                        entityDescription.Animation,
                        Session.GetInstance().Map.EntityProperties.MaxAnubisMovementSpeed,
                        Session.GetInstance().Map);
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
