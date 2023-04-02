using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TombOfAnubis
{
    public static class GameLogic
    {
        public static float deltaTime { get; set; }
        public static GameTime gameTime {get; set; }
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
            }
        }
        public static void OnCollision(Character p1, Character p2)
        {

            Transform t1 = p1.GetComponent<Transform>();
            Transform t2 = p2.GetComponent<Transform>();

            Vector2 center1 = p1.GetComponent<RectangleCollider>().CenterPosition;
            Vector2 center2 = p2.GetComponent<RectangleCollider>().CenterPosition;

            Vector2 overlap_direction = center2 - center1;
            overlap_direction.Normalize();

            t1.Position -= overlap_direction * deltaTime * 50;
            t2.Position += overlap_direction * deltaTime * 50;

        }
        public static void OnCollision(Character character, Wall wall)
        {
            WallCollision(character, wall);
        }
        public static void OnCollision(Character character, Artefact artefact)
        {
            int playerID = character.GetComponent<Player>().PlayerID;
            if (playerID == artefact.GetComponent<Player>().PlayerID) //if the player is the owner of the artefact, add it to the inventory and remove it from the map
            {
                character.GetComponent<Inventory>().AddArtefact();
                artefact.Delete();
                Console.WriteLine("Player " + playerID + " collected an artefact!");
                return;
            }

            //else: collide with artefact

            WallCollision(character, artefact);

        }
        public static void OnCollision(Character character, Dispenser dispenser)
        {
            dispenser.TryGiveItem(character.GetComponent<Inventory>(), gameTime.TotalGameTime.TotalSeconds);

            WallCollision(character, dispenser);
        }

        public static void OnCollision(Character character, Anubis anubis)
        {
            WallCollision(character, anubis); //treat Anubis like a wall (i.e. he is so much stronger than the player that he can push the player, but the player cannot push him)
        }

        public static void OnCollision(Anubis anubis, Wall wall)
        {
            WallCollision(anubis, wall);
        }

        public static void OnCollision(Character character, Altar altar)
        {
            PlaceArtefactIfPossible(character, altar);

            //can optionally treat the altar like a wall
            WallCollision(character, altar);
        }

        public static void WallCollision(Transform actorTransform, RectangleCollider actorCollider, RectangleCollider wallCollider)
        {
            float epsilon = 1e-1f; //additional offset to ensure the actor is actually outside of the wall

            float sum_half_widths = actorCollider.Size.X / 2f + wallCollider.Size.X / 2f;
            float sum_half_heights = actorCollider.Size.Y / 2f + wallCollider.Size.Y / 2f;

            Vector2 overlap = actorCollider.CenterPosition - wallCollider.CenterPosition; //IMPORTANT: Center difference and top-left-corner difference is NOT necessarily the same because the boxes don't have to be quadratic (stupid error that cost me like 3 hours)

            while (overlap.Length() == 0) //prevent NaNs
            {
                Random random = new Random();
                overlap.X = (float)random.NextDouble() - 0.5f;
                overlap.Y = (float)random.NextDouble() - 0.5f;
            }

            if (MathF.Abs(overlap.X / sum_half_widths) > MathF.Abs(overlap.Y / sum_half_heights))
            {
                overlap.X = MathF.Sign(overlap.X) * (sum_half_widths - MathF.Abs(overlap.X) + epsilon); //push out so much that the overlap is zero
                overlap.Y = 0;
                actorTransform.Position += overlap;
                //Console.WriteLine("X was bigger. Overlap adjustment: " + overlap.ToString());
            }
            else
            {
                overlap.X = 0;
                overlap.Y = MathF.Sign(overlap.Y) * (sum_half_heights - MathF.Abs(overlap.Y) + epsilon); //push out so much that the overlap is zero
                actorTransform.Position += overlap;
                //Console.WriteLine("Y was bigger. Overlap adjustment: " + overlap.ToString());
            }
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

                float artefactScale = 0.075f;
                Texture2D artefactTexture = Session.GetInstance().Map.Artefacts[playerID].Texture;
                float artefactWidth = artefactTexture.Width * artefactScale;

                Artefact artefact = new Artefact(playerID, new Vector2(playerID * artefactWidth, 0), Vector2.One * artefactScale, artefactTexture, false);
                altar.AddChild(artefact);
                Console.WriteLine("Artefact of player " + playerID + " was placed!");
            }
            else if (!altarInventory.ArtefactSlotsFull())
            {
                Console.WriteLine("Either player " + playerID + " doesn't have an artefact or their artefact is already placed!");
            }

            if (altarInventory.ArtefactSlotsFull())
            {
                Console.WriteLine("All artefacts placed! Anubis was defeated!");
            }
            else
            {
                Console.WriteLine("You need " + (Session.GetInstance().NumberOfPlayers - altarInventory.ArtefactCount()) + " more artefacts!");
            }


        }
        public static void WallCollision(Entity blockedEntity, Entity wall){
            Transform t1 = blockedEntity.GetComponent<Transform>();

            RectangleCollider c1 = blockedEntity.GetComponent<RectangleCollider>();
            RectangleCollider c2 = wall.GetComponent<RectangleCollider>();

            WallCollision(t1, c1, c2);
        }
    }
}
