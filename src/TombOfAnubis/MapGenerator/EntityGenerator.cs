using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static TombOfAnubis.Character;

namespace TombOfAnubis
{
    public static class EntityGenerator
    {
        private static ContentManager content;
        public static void Initialize(ContentManager _content)
        {
            content = _content;
        }
        public static List<Entity> GenerateEntities(List<EntityDescription> EntityDescriptions) 
        {
            //Session.GetInstance().Map.Artefacts.Clear();

            //TODO: Generate entities, Fill out Map.Artefacts etc
            List<Entity> entities = new List<Entity>();

            foreach (EntityDescription entityDescription in EntityDescriptions)
            {
                Type t = Type.GetType(entityDescription.ClassName);
                if(t == typeof(Character))
                {
                    Enum.TryParse(entityDescription.Type, out CharacterType type);
                    if ((int)type < Session.GetInstance().NumberOfPlayers)
                    entities.Add(SpawnCharacter(entityDescription));
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
            entityDescription.Load(content, @"Textures\Characters");
            Enum.TryParse(entityDescription.Type, out CharacterType type);
            return new Character(
                type,
                Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                entityDescription.Scale,
                entityDescription.Texture,
                Session.GetInstance().Map.EntityProperties.MaxCharacterMovementSpeed,
                entityDescription.Animation,
                entityDescription.Animation
                );
        }
        public static Entity SpawnArtefact(EntityDescription entityDescription)
        {
            entityDescription.Load(content, @"Textures\Objects\Artefacts");
            Enum.TryParse(entityDescription.Type, out CharacterType type);

            return new Artefact(
                    (int)type,
                    Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                    entityDescription.Scale,
                    entityDescription.Scale * 10,
                    entityDescription.Texture,
                    true
                    );
        }
        public static Entity SpawnDispenser(EntityDescription entityDescription)
        {
            return null;
        }
        public static Entity SpawnAnubis(EntityDescription entityDescription)
        {
            entityDescription.Load(content, @"Textures\Characters");
            return new Anubis(
                        Session.GetInstance().Map.CreateEntityTileCenteredPosition(entityDescription),
                        entityDescription.Scale,
                        entityDescription.Texture,
                        entityDescription.Animation,
                        Session.GetInstance().Map.EntityProperties.MaxAnubisMovementSpeed,
                        Session.GetInstance().Map);
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
            return null;
        }
        public static Entity SpawnButton(EntityDescription entityDescription)
        {
            return null;
        }
    }
}
// Characters
//map.Characters = input.ReadObject<List<EntityDescription>>();
//foreach(EntityDescription ed in map.Characters)
//{
//    ed.Load(input.ContentManager, @"Textures\Characters");
//}

//// Anubis
//map.Anubis = input.ReadObject<EntityDescription>();
//map.Anubis.Load(input.ContentManager, @"Textures\Characters");
//map.Anubis.Texture = input.ContentManager.Load<Texture2D>(
//        Path.Combine(@"Textures\Characters",
//        map.Anubis.SpriteTextureName));

//// Artefacts
//map.Artefacts = input.ReadObject<List<EntityDescription>>();
//foreach (EntityDescription ed in map.Artefacts)
//{
//    ed.Load(input.ContentManager, @"Textures\Objects\Artefacts");

//}

//// Altar
//map.Altar = input.ReadObject<EntityDescription>();
//map.Altar.Load(input.ContentManager, @"Textures\Objects\Altar");

//// Dispensers
//map.Dispensers = input.ReadObject<List<EntityDescription>>();
//foreach (EntityDescription ed in map.Dispensers)
//{
//    ed.Load(input.ContentManager, @"Textures\Objects\Dispensers");
//}

//// Traps
//map.Traps = input.ReadObject<List<EntityDescription>>();
//foreach(EntityDescription ed in map.Traps)
//{
//    ed.Load(input.ContentManager, @"Textures\Objects\Traps");
//}

//// Buttons
//map.Buttons = input.ReadObject<List<EntityDescription>>();
//foreach (EntityDescription ed in map.Buttons)
//{
//    ed.Load(input.ContentManager, @"Textures\Objects\Buttons");
//}

// Fist
//map.Fist = input.ReadObject<EntityDescription>();
//map.Fist.Load(input.ContentManager, @"Textures\Objects\Items");


//for (int i = 0; i < gameStartDescription.NumberOfPlayers; i++)
//{
//    EntityDescription character = singleton.Map.Characters[i];
//    singleton.World.AddChild(new Character(
//        i,
//        singleton.Map.CreateEntityTileCenteredPosition(character),
//        character.Scale,
//        character.Texture,
//        singleton.Map.EntityProperties.MaxCharacterMovementSpeed,
//        character.Animation,
//        character.Animation
//        ));

//    EntityDescription artefact = singleton.Map.Artefacts[i];
//    singleton.World.AddChild(new Artefact(
//        i,
//        singleton.Map.CreateEntityTileCenteredPosition(artefact),
//        artefact.Scale,
//        artefact.Scale * 10,
//        artefact.Texture,
//        true
//        ));
//}

//DEBUG: Attach ParticleEmitter to first character
/*
ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
pec.LocalPosition = Vector2.Zero;
pec.RandomizedSpawnPositionRadius = 20f;
//doesn't work yet
pec.ParticlesMoveWithEntity = false;
pec.Texture = ParticleTextureLibrary.BasicParticle;
pec.RandomizedTintMin = Color.LightGray;
pec.RandomizedTintMax = Color.DarkGray;
pec.Scale = Vector2.One * 0.2f;
pec.ScalingMode = ScalingMode.LinearDecreaseToZero;
pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
pec.EmitterDuration = 0f;
pec.ParticleDuration = 1f;
pec.EmissionFrequency = 60f;
pec.EmissionRate = 2f;
pec.InitialSpeed = 100f;
pec.SpawnDirection = new Vector2(0f, -1f);
pec.SpawnConeDegrees = 90f;
pec.Gravity = new Vector2(0f, 0f);
//currently behaves a bit unintuitively
pec.LocalPointForcePosition = Vector2.Zero;
pec.PointForceStrength = 0f;
pec.PointForceUsesQuadraticFalloff = false;
pec.Drag = 0.5f;

singleton.Scene.GetChildrenOfType<Character>()[0].AddComponent(new ParticleEmitter(pec));*/

//foreach ( var dispenser in singleton.Map.Dispensers ) {
//    _ = Enum.TryParse(dispenser.Type, out DispenserType type);
//    singleton.World.AddChild(new Dispenser(
//        singleton.Map.CreateEntityTileCenteredPosition(dispenser),
//        dispenser.Scale,
//        dispenser.Texture,
//        type
//        ));
//}

//singleton.World.AddChild(new Anubis(
//                        singleton.Map.CreateEntityTileCenteredPosition(singleton.Map.Anubis),
//                        singleton.Map.Anubis.Scale,
//                        singleton.Map.Anubis.Texture,
//                        singleton.Map.Anubis.Animation,
//                        singleton.Map.EntityProperties.MaxAnubisMovementSpeed,
//                        singleton.Map));

//add smoke particle effect to Anubis so it seems more like he's floating on a cloud
//ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
//pec.LocalPosition = new Vector2(25f, 100f);
//pec.RandomizedSpawnPositionRadius = 50f;
////doesn't work yet
//pec.ParticlesMoveWithEntity = false;
//pec.Texture = ParticleTextureLibrary.BasicParticle;
//pec.SpriteLayer = 1;
//pec.InitialAlpha = 0.5f;
//pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
//pec.RandomizedTintMin = Color.SlateGray;
//pec.RandomizedTintMax = Color.DimGray;
//pec.Scale = Vector2.One * 0.4f;
//pec.ScalingMode = ScalingMode.Constant;
//pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
//pec.EmitterDuration = 0f;
//pec.ParticleDuration = 2f;
//pec.EmissionFrequency = 60f;
//pec.EmissionRate = 1f;
//pec.InitialSpeed = 50f;
//pec.SpawnDirection = new Vector2(0f, -1f);
//pec.SpawnConeDegrees = 90f;
//pec.Gravity = new Vector2(0f, 0f);
////currently behaves a bit unintuitively
//pec.LocalPointForcePosition = Vector2.Zero;
//pec.PointForceStrength = 0f;
//pec.PointForceUsesQuadraticFalloff = false;
//pec.Gravity = new Vector2(0f, 0f);
//pec.Drag = 0.5f;

//singleton.World.GetChildrenOfType<Anubis>()[0].AddComponent(new ParticleEmitter(pec));


//singleton.World.AddChild(new Altar(
//                        singleton.Map.CreateEntityTileCenteredPosition(singleton.Map.Altar),
//                        singleton.Map.Altar.Scale,
//                        singleton.Map.Altar.Texture,
//                        singleton.NumberOfPlayers));

//foreach(var trap in singleton.Map.Traps)
//{
//    _ = Enum.TryParse(trap.Type, out TrapType type);
//    singleton.World.AddChild(new Trap(
//        type,
//        singleton.Map.CreateEntityTileCenteredPosition(trap),
//        trap.Scale,
//        trap.Texture,
//        trap.Animation
//        ));
//}

//foreach(var button in singleton.Map.Buttons)
//{
//    List<Vector2> connectedTraps = new List<Vector2>();

//    foreach(EntityDescription trapEntity in button.ConnectedTrapPositions)
//    {
//        connectedTraps.Add(singleton.Map.CreateEntityTileCenteredPositionSpriteless(trapEntity));
//    }
//    _ = Enum.TryParse(button.Type, out ButtonType type);
//    singleton.World.AddChild(new Button(
//        type,
//        singleton.Map.CreateEntityTileCenteredPosition(button),
//        button.Scale,
//        button.Texture,
//        button.Animation,
//        connectedTraps
//        ));
//}