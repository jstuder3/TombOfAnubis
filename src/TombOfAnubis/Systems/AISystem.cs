﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class AISystem : BaseSystem<AI>
    {
        public Scene Scene { get; set; }
        public AISystem(Scene scene)
        {
            Scene = scene;
        }

        protected enum Directions
        {
            Up,
            Right,
            Down,
            Left
        }

        // Initialize direction to invalid direction
        public int MovingDirection { get; set; } = -1;
        public int NumStepsInSameDirection { get; set; } = 0;
        public int MaxStepsInSameDirection { get; set; } = 0;

        private int MaxTailDistance { get; set; } = 10;

        public Vector2 PreviousPosition { get; set; } = Vector2.Zero;

        public int TimesUnchangedPosition { get; set; } = 0;
        public const int MaxTimesUnchangedPosition = 3;

        public override void Update(GameTime deltaTime)
        {

            //Console.WriteLine("start update AnubisAISystem");
            Random rnd = new Random();

            foreach (AI ai in components)
            {
                //Console.WriteLine("reached anubis section");
                Entity entity = ai.Entity;
                Transform transform = entity.GetComponent<Transform>();
                Movement movement = entity.GetComponent<Movement>();

                MovementGraph movementGraph = ai.MovementGraph;
                List<Character> characters = Scene.GetChildrenOfType<Character>();
                List<Artefact> artefacts = Scene.GetChildrenOfType<Artefact>();

                RectangleCollider collider = entity.GetComponent<RectangleCollider>();

                // Node anubis = movementGraph.GetNode(entity)
                // Node c1 = movementGraph.GetNode(characters[0])
                // List<Vector2> currentPath = movementGraph.CreatePath(anubis, c1)
                // Vector2 currentTarget = currentPath[0]

                // movement.IsWalking = false;
                float deltaTimeSeconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;
                Vector2 newPosition = transform.Position;

                bool random_walk = false;

                if (random_walk)
                {
                    int numDirections = Enum.GetNames(typeof(Directions)).Length;
                    // Initializes Anubis' first moving direction
                    int newDirection = (MovingDirection != -1) ? MovingDirection : rnd.Next(numDirections);

                    PreviousPosition = (PreviousPosition.Equals(Vector2.Zero)) ? transform.Position : PreviousPosition;
                    TimesUnchangedPosition = (PreviousPosition.Equals(transform.Position)) ? TimesUnchangedPosition + 1 : 0;
                    PreviousPosition = transform.Position;

                    //Console.Write("ANUBIS: ");
                    //Console.WriteLine(TimesUnchangedPosition + "\t" + PreviousPosition + "\t" + transform.Position);
                    //Console.WriteLine(NumStepsInSameDirection + "\t" + MaxStepsInSameDirection);

                    // while (collider.BlockedDirections.Contains((BlockDirections)newDirection) && newDirection == MovingDirection)
                    if ((NumStepsInSameDirection == MaxStepsInSameDirection || TimesUnchangedPosition >= MaxTimesUnchangedPosition))
                    {
                        while (newDirection == MovingDirection)
                        {
                            //Console.WriteLine("Collision: Determining new direction...");
                            newDirection = rnd.Next(numDirections);
                        }
                        NumStepsInSameDirection = 0;
                        MaxStepsInSameDirection = rnd.Next(60, 100);

                    }

                    // Console.WriteLine("New direction is " + (Directions)newDirection);
                    MovingDirection = newDirection;

                    if (!movement.IsTrapped)
                    {
                        newPosition = transform.Position;

                        if (newDirection == (int)Directions.Up)
                        {
                            //walk up
                            newPosition.Y -= movement.MaxSpeed * deltaTimeSeconds;
                            movement.IsWalking = true;
                            movement.Orientation = Orientation.Up;

                        }
                        else if (newDirection == (int)Directions.Down)
                        {
                            // walk down 
                            newPosition.Y += movement.MaxSpeed * deltaTimeSeconds;
                            movement.IsWalking = true;
                            movement.Orientation = Orientation.Down;
                        }
                        else if (newDirection == (int)Directions.Left)
                        {
                            //walk left
                            newPosition.X -= movement.MaxSpeed * deltaTimeSeconds;
                            movement.IsWalking = true;
                            movement.Orientation = Orientation.Left;
                        }
                        else if (newDirection == (int)Directions.Right)
                        {
                            //walk right 
                            newPosition.X += movement.MaxSpeed * deltaTimeSeconds;
                            movement.IsWalking = true;
                            movement.Orientation = Orientation.Left;
                        }
                        else
                        {
                            Console.WriteLine("Error: Unknown direction " + newDirection);
                            movement.IsWalking = false;
                        }
                    }
                }
                else
                {
                    int anubis_node_id = movementGraph.position_to_node_id(transform.Position);
                    //int anubis_node_id = 5;

                    //Console.WriteLine("using real anubis AI");
                    newPosition = transform.Position;
                    if (!movement.IsTrapped)
                    {
                        //loop over all players and get closest player if he is close enough (<=MaxTailDistance)
                        int closest_player_dist = MaxTailDistance + 1;
                        bool tailed_a_player = false;
                        int tailing_player_nr = -1;

                        foreach (Character player in characters)
                        {
                            Transform cur_player_transform = player.GetComponent<Transform>();
                            int cur_player_node_id = movementGraph.position_to_node_id(cur_player_transform.Position);

                            //Console.WriteLine("test, player id: " + player.Id + ", position: "+ cur_player_transform.Position.X +"," + cur_player_transform.Position.Y);
                            if (cur_player_node_id >= 0 && Math.Abs(anubis_node_id - cur_player_node_id) < closest_player_dist)
                            {
                                tailed_a_player = true;
                                closest_player_dist = Math.Abs(anubis_node_id - cur_player_node_id);
                                tailing_player_nr = player.GetComponent<Player>().PlayerID;
                            }
                        }

                        if (tailed_a_player)
                        {
                            //Console.WriteLine("Anubis tailes player " + tailing_player_nr);
                        }
                    }
                }
                //if (currentActions.Contains(PlayerActions.UseObject))
                //{
                //    //check which objects are currently colliding with the player. then check that they are in the orientation the player is looking in
                //    // if the targeted object is iteractable (i.e. an artefact or an item dispenser, or anything else that can be interacted with), trigger the corresponding interaction

                //    //if item dispenser: give player an item of the type corresponding to the dispenser, assuming the player can carry such an item

                //    //if a button: trigger the interaction corresponding to that button

                //    //if a player: check if trapped/unconscious, then check if the current player can free/resurrect that player

                //}
                NumStepsInSameDirection += 1;
                transform.Position = newPosition;

                return;

                
            }
        }
    }
}
