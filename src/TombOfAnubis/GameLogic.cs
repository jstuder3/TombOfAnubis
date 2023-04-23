﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TombOfAnubisContentData;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TombOfAnubis
{
    public static class GameLogic
    {
        public static float DeltaTime { get; set; }
        public static GameTime GameTime {get; set; }

        private static Random random = new Random();
        public static void OnCollision(Entity source, Entity target)
        {
            //check that the source and target still have a collider (they might have been destroyed)
            switch (source.GetType().Name, target.GetType().Name)
            {
                case (nameof(Character), nameof(Character)):
                    OnCollision((Character)source, (Character)target);
                    break;
                case (nameof(Character), nameof(Wall)):
                    OnCollision((Character)source, (Wall)target);
                    break;
                case (nameof(Wall), nameof(Character)):
                    OnCollision((Character)target, (Wall)source);
                    break;
                case (nameof(Character), nameof(Artefact)):
                    OnCollision((Character)source, (Artefact)target);
                    break;
                case (nameof(Artefact), nameof(Character)):
                    OnCollision((Character)target, (Artefact)source);
                    break;
                case (nameof(Character), nameof(Dispenser)):
                    OnCollision((Character)source, (Dispenser)target);
                    break;
                case (nameof(Dispenser), nameof(Character)):
                    OnCollision((Character)target, (Dispenser)source);
                    break;
                case (nameof(Character), nameof(Anubis)):
                    OnCollision((Character)source, (Anubis)target);
                    break;
                case (nameof(Anubis), nameof(Character)):
                    OnCollision((Character)target, (Anubis)source);
                    break;
                case (nameof(Anubis), nameof(Wall)):
                    OnCollision((Anubis)source, (Wall)target);
                    break;
                case (nameof(Wall), nameof(Anubis)):
                    OnCollision((Wall)target, (Anubis)source);
                    break;
                case (nameof(Character), nameof(Altar)):
                    OnCollision((Character)source, (Altar)target);
                    break;
                case (nameof(Altar), nameof(Character)):
                    OnCollision((Character)target, (Altar)source);
                    break;
                case (nameof(Fist), nameof(Anubis)):
                    OnCollision((Fist)source, (Anubis)target);
                    break;
                case (nameof(Anubis), nameof(Fist)):
                    OnCollision((Fist)target, (Anubis)source);
                    break;
                case (nameof(Character), nameof(Trap)):
                    OnCollision((Character)source, (Trap)target);
                    break;
                case(nameof(Trap), nameof(Character)):
                    OnCollision((Character)target, (Trap)source);
                    break;
                case (nameof(Character), nameof(Button)):
                    OnCollision((Character)source, (Button)target);
                    break;
                case(nameof(Button), nameof(Character)):
                    OnCollision((Character)target, (Button)source);
                    break;
            }
        }
        public static void OnCollision(Character character1, Character character2)
        {

            //if neither character is trapped, push them apart
            if (!character1.GetComponent<Movement>().IsTrapped() && !character2.GetComponent<Movement>().IsTrapped())
            {

                Transform t1 = character1.GetComponent<Transform>();
                Transform t2 = character2.GetComponent<Transform>();

                Vector2 center1 = character1.GetComponent<RectangleCollider>().GetCenter();
                Vector2 center2 = character2.GetComponent<RectangleCollider>().GetCenter();

                Vector2 overlap_direction = center2 - center1;
                overlap_direction.Normalize();

                t1.Position -= overlap_direction * DeltaTime * 100;
                t2.Position += overlap_direction * DeltaTime * 100;
            }

            //if both are trapped, do nothing
            else if (character1.GetComponent<Movement>().IsTrapped() && character2.GetComponent<Movement>().IsTrapped()) { }
            //if at least one character is trapped, check if one can revive the other
            else if (!character1.GetComponent<Movement>().IsTrapped() && character2.GetComponent<Movement>().IsTrapped() && character1.GetComponent<Inventory>().HasResurrectItem())
            {
                InventorySlot slot = character1.GetComponent<Inventory>().GetResurrectionSlot();
                slot.ClearItem();

                character2.GetComponent<Movement>().State = MovementState.Idle;
                character2.GetComponent<Animation>()?.SetActiveClip(AnimationClipType.Idle);

                ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
                pec.LocalPosition = new Vector2(30f, 30f);
                pec.RandomizedSpawnPositionRadius = 40f;
                pec.Texture = ParticleTextureLibrary.PlusFilledWtihOutline;
                pec.SpriteLayer = 3;
                pec.RandomizedTintMin = Color.Chartreuse;
                pec.RandomizedTintMax = Color.Green;
                pec.Scale = Vector2.One * 0.4f;
                pec.ScalingMode = ScalingMode.LinearDecreaseToZero;
                pec.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
                pec.EmitterDuration = 1f;
                pec.ParticleDuration = 1f;
                pec.EmissionFrequency = 30f;
                pec.EmissionRate = 1f;
                pec.InitialSpeed = 40f;
                pec.SpawnDirection = new Vector2(0f, -1f);
                pec.SpawnConeDegrees = 360f;
                pec.Drag = 0.5f;

                character2.AddComponent(new ParticleEmitter(pec));


            }
            else if (character1.GetComponent<Movement>().IsTrapped() && !character2.GetComponent<Movement>().IsTrapped() && character2.GetComponent<Inventory>().HasResurrectItem())
            {
                InventorySlot slot = character2.GetComponent<Inventory>().GetResurrectionSlot();
                slot.ClearItem();

                character1.GetComponent<Movement>().State = MovementState.Idle;
                character1.GetComponent<Animation>()?.SetActiveClip(AnimationClipType.Idle);

                ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
                pec.LocalPosition = new Vector2(30f, 30f);
                pec.RandomizedSpawnPositionRadius = 40f;
                pec.Texture = ParticleTextureLibrary.PlusFilledWtihOutline;
                pec.SpriteLayer = 3;
                pec.RandomizedTintMin = Color.Chartreuse;
                pec.RandomizedTintMax = Color.Green;
                pec.Scale = Vector2.One * 0.4f;
                pec.ScalingMode = ScalingMode.LinearDecreaseToZero;
                pec.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
                pec.EmitterDuration = 1f;
                pec.ParticleDuration = 1f;
                pec.EmissionFrequency = 30f;
                pec.EmissionRate = 1f;
                pec.InitialSpeed = 40f;
                pec.SpawnDirection = new Vector2(0f, -1f);
                pec.SpawnConeDegrees = 360f;
                pec.Drag = 0.5f;

                character1.AddComponent(new ParticleEmitter(pec));

            }



        }
        public static void OnCollision(Character character, Wall wall)
        {
            StaticCollision(character, wall);
        }
        public static void OnCollision(Character character, Artefact artefact)
        {
            int playerID = character.GetComponent<Player>().PlayerID;
            if (playerID == artefact.GetComponent<Player>().PlayerID) //if the player is the owner of the artefact, add it to the inventory and remove it from the map
            {
                character.GetComponent<Inventory>().AddArtefact();
                artefact.Delete();
                AudioController.PlaySoundEffect("artefactPickup");
                Console.WriteLine("Player " + playerID + " collected an artefact!");
                return;
            }

            //else: collide with artefact

            StaticCollision(character, artefact);

        }

        public static void OnCollision(Character character, Dispenser dispenser)
        {
            bool newItem = dispenser.TryGiveItem(character.GetComponent<Inventory>(), GameTime.TotalGameTime.TotalSeconds);
            if (newItem)
            {
                AudioController.PlaySoundEffect("itemPickup");
            }

            StaticCollision(character, dispenser);
        }

        public static void OnCollision(Character character, Anubis anubis)
        {

            if (!character.GetComponent<Movement>().IsTrapped())
            {
                AudioController.PlaySoundEffect("anubisRoar");
            }

            character.GetComponent<Movement>().State = MovementState.Trapped;
            character.GetComponent<Animation>()?.SetActiveClip(AnimationClipType.Dead);
           // AISystem.DetailAPlayer(character);

            foreach(ParticleEmitter pe in character.GetComponentsOfType<ParticleEmitter>())
            {
                pe.EndEmitter();
            }

            /*if(character.GetComponent<Movement>().CanMove()) //use CanMove instead of IsVisibleToAnubis(), because when the player is invisible, there should still be a collision
            {
                StaticCollision(character, anubis); //treat Anubis like a wall (i.e. he is so much stronger than the player that he can push the player, but the player cannot push him)
            }*/

            bool gameover = true;
            foreach (Character ch in Session.GetInstance().World.GetChildrenOfType<Character>())
            {
                if (!ch.GetComponent<Movement>().IsTrapped())
                {
                    gameover = false;
                }
            }
            if (gameover)
            {
                Session.GetInstance().SessionState = SessionState.GameOver;
            }
        }

        public static void OnCollision(Anubis anubis, Wall wall)
        {
            StaticCollision(anubis, wall);
        }

        public static void OnCollision(Character character, Altar altar)
        {
            PlaceArtefactIfPossible(character, altar);

            //can optionally treat the altar like a wall
            StaticCollision(character, altar);
        }

        public static void OnCollision(Fist fist, Anubis anubis)
        {
            //When Anubis collides with the fist, the fist is destroyed (by ending its lifetime GameEffect), a smoke effect is spawned and Anubis is stunned for 2 seconds

            Session singleton = Session.GetInstance();
            //VFX vfx = new VFX(anubis.GetComponent<Transform>().Position, Fist.Scale * 2f, Fist.Texture, Fist.AnimationClipList, 2, AnimationClipType.VFX_01);
            //singleton.Scene.AddChild(vfx);
            //vfx.AddComponent(new GameplayEffect(EffectType.Lifetime, 0.5f));

            foreach(GameplayEffect gameplayEffect in fist.GetComponentsOfType<GameplayEffect>())
            {
                gameplayEffect.EndGameplayEffect();
            }

            anubis.AddComponent(new GameplayEffect(EffectType.Stunned, 2f, Visibility.Game));

            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(30f, 30f);
            pec.RandomizedSpawnPositionRadius = 40f;
            pec.Texture = ParticleTextureLibrary.Circle;
            pec.SpriteLayer = 3;
            pec.RandomizedTintMin = Color.Yellow;
            pec.RandomizedTintMax = Color.LightYellow;
            pec.Scale = Vector2.One * 0.4f;
            pec.ScalingMode = ScalingMode.LinearDecreaseToZero;
            pec.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
            pec.EmitterDuration = 2f;
            pec.ParticleDuration = 1f;
            pec.EmissionFrequency = 30f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 10f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 360f;
            pec.Drag = 0.5f;

            anubis.AddComponent(new ParticleEmitter(pec));

        }

        public static void OnCollision(Character character, Trap trap)
        {
            if (trap.IsEnabled())
            {
                StaticCollision(character, trap);
            }
        }

        public static void OnCollision(Character character, Button button)
        {
            //do nothing; all of this is handled in the ButtonController
        }

        public static void PlaceArtefactIfPossible(Character character, Altar altar)
        {
            if (character == null || altar == null) return;

            int playerID = character.GetComponent<Player>().PlayerID;
            Inventory characterInventory = character.GetComponent<Inventory>();
            Inventory altarInventory = altar.GetComponent<Inventory>();


            if (characterInventory.HasArtefact() && !altarInventory.HasArtefact(playerID))
            {

                characterInventory.ClearArtefactSlots();
                altarInventory.AddArtefact(playerID);

                float artefactScale = 0.015f;
                Texture2D artefactTexture = Session.GetInstance().Map.Artefacts[playerID].Texture;

                Vector2 altarSize = altar.SpriteSize();
                float artefactWidth = artefactTexture.Width * artefactScale;

                float w = altarSize.X;
                float h = altarSize.Y;
                Vector2[] artefactPositions = new Vector2[]
                {
                    new Vector2(0.06f*w, -0.1f*h),
                    new Vector2(0.91f*w - artefactWidth, -0.1f*h),
                    new Vector2(0.04f*w, 0.2f*h),
                    new Vector2(0.95f*w - artefactWidth, 0.2f*h)
                };

                Artefact artefact = new Artefact(playerID, artefactPositions[playerID], Vector2.One * artefactScale, artefactTexture, false);
                altar.AddChild(artefact);
                Console.WriteLine("Artefact of player " + playerID + " was placed!");
                AudioController.PlaySoundEffect("artefactPlaced");
            }
            else if (!altarInventory.ArtefactSlotsFull())
            {
                Console.WriteLine("Either player " + playerID + " doesn't have an artefact or their artefact is already placed!");
            }

            if (altarInventory.ArtefactSlotsFull())
            {
                Console.WriteLine("All artefacts placed! Anubis was defeated!");
                Session.GetInstance().SessionState = SessionState.GameWon;
            }
            else
            {
                Console.WriteLine("You need " + (Session.GetInstance().NumberOfPlayers - altarInventory.ArtefactCount()) + " more artefacts!");
            }


        }
        public static void StaticCollision(Entity blockedEntity, Entity staticObject){
            Transform actorTransform = blockedEntity.GetComponent<Transform>();

            RectangleCollider actorCollider = blockedEntity.GetComponent<RectangleCollider>();
            RectangleCollider wallCollider = staticObject.GetComponent<RectangleCollider>();

            float epsilon = 1e0f; //additional offset to ensure the actor is actually outside of the wall

            float sum_half_widths = actorCollider.Size.X / 2f + wallCollider.Size.X / 2f;
            float sum_half_heights = actorCollider.Size.Y / 2f + wallCollider.Size.Y / 2f;

            Vector2 overlap = actorCollider.GetCenter() - wallCollider.GetCenter(); //IMPORTANT: Center difference and top-left-corner difference is NOT necessarily the same because the boxes don't have to be quadratic (stupid error that cost me like 3 hours)

            while (overlap.Length() == 0) //prevent NaNs
            {
                overlap.X = (float)random.NextDouble() - 0.5f;
                overlap.Y = (float)random.NextDouble() - 0.5f;
            }

            // Check whether first to push out in x or y direction based on which overlap is bigger.
            // We can't move in both directions at the same time because every overlap in one direction necessarily brings with it an overlap in the other direction,
            // which would cause the actor to be "teleported" unintentionally.
            // As an approximation, we only move in the direction with the bigger overlap.
            // Unfortunately, this brings with it some issues when the player touches multiple walls at the same time, as the behaviour is dependent on the order in which the collisions are handled

            // This is why we do the following:
            // In case of almost equality we simply don't know where we should move the colliding object. So we add it to a list of skipped collisions
            // and hope someone else solves our problems for us. Otherwise we will just pick one option when we deal with this collision the next time (Corner on corner collisions).
            if(MathF.Abs(MathF.Abs(overlap.X / sum_half_widths) - MathF.Abs(overlap.Y / sum_half_heights)) < 0.01f){
                if(!CollisionSystem.SkippedCollisions.Contains(new Tuple<Collider, Collider>(actorCollider, wallCollider)))
                {
                    CollisionSystem.SkippedCollisions.Add(new Tuple<Collider, Collider>(actorCollider, wallCollider));
                    return;
                }
            }

            if (MathF.Abs(overlap.X / sum_half_widths) > MathF.Abs(overlap.Y / sum_half_heights))
            {
                overlap.X = MathF.Sign(overlap.X) * (sum_half_widths - MathF.Abs(overlap.X) + epsilon); //push out so much that the overlap is zero
                overlap.Y = 0;
            }
            else
            {
                overlap.X = 0;
                overlap.Y = MathF.Sign(overlap.Y) * (sum_half_heights - MathF.Abs(overlap.Y) + epsilon); //push out so much that the overlap is zero
            }
          
            //execute overlap correction
            actorTransform.Position += overlap;
            actorCollider.Position += overlap;
        }
    }
}
