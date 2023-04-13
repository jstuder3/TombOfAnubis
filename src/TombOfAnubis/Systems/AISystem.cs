using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using QuikGraph;
using System;
using System.Collections.Generic;
//using System.Numerics;

namespace TombOfAnubis
{
    public enum AnubisBehaviour
    {
        Random,
        TailPlayers,
        TrueAI
    }

    public class AISystem : BaseSystem<AI>
    {
        public Scene Scene { get; set; }

        public AnubisBehaviour AnubisBehaviour { get; set; }
        public AISystem(Scene scene, AnubisBehaviour anubisBehaviour)
        {
            Scene = scene;
            AnubisBehaviour = anubisBehaviour;
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


        public bool tailingPlayer = false;
        public int tailedPlayerId = default;
        Character tailedPlayer = default;

        public void printState(AI ai)
        {
            Entity entity = ai.Entity;
            Transform transform = entity.GetComponent<Transform>();
            List<Character> characters = Scene.GetChildrenOfType<Character>();

            //Anubis:
            Vector2 positionAnubis = transform.Position;
            int nodeIdAnubis = ai.MovementGraph.ToNodeID(positionAnubis);

            Console.WriteLine("----Print AI System State:------");
            Console.WriteLine("Anubis: position, nodeId: " + positionAnubis + ", " + nodeIdAnubis);

            foreach (Character player in characters)
            {
                Transform transformPlayer = player.GetComponent<Transform>();
                Vector2 positionPlayer = transformPlayer.Position;
                int nodeIdPlayer = ai.MovementGraph.ToNodeID(transformPlayer.Position);
                Console.WriteLine("Player nr: " + player.GetComponent<Player>().PlayerID + ", position " + positionPlayer + ", nodeId " + nodeIdPlayer + ", distance: " + ai.MovementGraph.GetDistance(nodeIdAnubis, nodeIdPlayer));
            }


        }

        private Vector2 getDirection(AI ai, Vector2 anubisPosition, Vector2 playerPosition)
        {

            int nodeIdAnubis = ai.MovementGraph.ToNodeID(anubisPosition);
            int nodeIdPlayer = ai.MovementGraph.ToNodeID(playerPosition);

            Point tileCoordinateAnubis = ai.MovementGraph.ToTileCoordinate(nodeIdAnubis);
            Point tileCoordinatePlayer = ai.MovementGraph.ToTileCoordinate(nodeIdPlayer);

            //if player&anubis close use real direction
            if (nodeIdAnubis == nodeIdPlayer || ai.MovementGraph.isTileNeighbor(tileCoordinateAnubis, tileCoordinatePlayer))
            {
                Vector2 difference = playerPosition - anubisPosition;
                Vector2 temp2 = difference; //only for debug
                difference.Normalize();
                //Console.WriteLine("State: direcToPlayer, anubis, player, target, direction, direction norml.: " + anubisPosition + ", " + playerPosition + ", " + playerPosition + ", " + temp2 + ", " + difference);
                return difference;
            }


            //else use direction to next tile\node of the path to the tailed player

            if (!ai.MovementGraph.PathExists(nodeIdAnubis, nodeIdPlayer))
            {
                Console.WriteLine("Error: Anbuis cound not find a Path to cur tailed player");

                //hotfix:
                this.tailingPlayer = false;
                return new Vector2(0, 0);
            }


            Vector2 target = ai.MovementGraph.GetTargetToWalkTo(nodeIdAnubis, nodeIdPlayer);
            Vector2 direction = target - anubisPosition;
            
            //somehow sometimes anubis position is already at target position??? try hotfix
            if(direction.LengthSquared() < 0.1)
            {
                Console.WriteLine("AI: movement HotFix needed, tile distance too small");
                if(ai.MovementGraph.GetDistance(nodeIdAnubis, nodeIdPlayer) > 1)
                {
                    target = ai.MovementGraph.getNthTargetToWalkTo(nodeIdAnubis, nodeIdPlayer, 2);
                    direction = target - anubisPosition;
                } else
                {
                    direction = playerPosition - anubisPosition;
                }
                
            }
            Vector2 temp = direction;
            direction.Normalize();
            //Console.WriteLine("State: direcToTile, anubis, player, target, direction, direciton norml.: " + anubisPosition + ", " + playerPosition + ", " + target + ", " + temp + ", " + direction);
            return direction;
        }

        public bool updateClosestPlayer(AI ai, Vector2 anubisPosition)
        {
            //Console.WriteLine("cur no tailed player, try to change now");
            MovementGraph movementGraph = ai.MovementGraph;
            List<Character> characters = Scene.GetChildrenOfType<Character>();


            int nodeIdAnubis = movementGraph.ToNodeID(anubisPosition);

            int closest_player_dist = 999999;
            bool tailed_a_player = false;
            int tailing_player_nr = -1;
            Character tailed_player = default;

            foreach (Character player in characters)
            {
                Transform cur_player_transform = player.GetComponent<Transform>();
                int nodeIdPlayer = movementGraph.ToNodeID(cur_player_transform.Position);

                if (!player.GetComponent<Movement>().IsTrapped() && ai.MovementGraph.PathExists(nodeIdAnubis, nodeIdPlayer))
                {

                    int dist = ai.MovementGraph.GetDistance(nodeIdAnubis, nodeIdPlayer);
                    if (nodeIdPlayer >= 0 && dist >= 0 && dist < closest_player_dist)
                    {
                        tailed_a_player = true;
                        closest_player_dist = dist;
                        tailing_player_nr = player.GetComponent<Player>().PlayerID;
                        tailed_player = player;
                    }
                }
            }

            if(tailed_a_player)
            {
                
                this.tailingPlayer = true;
                this.tailedPlayer = tailed_player;
                this.tailedPlayerId = tailing_player_nr;
                Console.WriteLine("AI: tailing new player: " + this.tailedPlayer.GetComponent<Player>().PlayerID);
                return true;
            }
            Console.WriteLine("AI: udpateClosestPlayer failed: could not find any reachable player");
            return false;
        }   

        public override void Update(GameTime deltaTime)
        {

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

                if (AnubisBehaviour.Random == this.AnubisBehaviour)
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

                    if (!movement.IsTrapped())
                    {
                        newPosition = transform.Position;

                        if (newDirection == (int)Directions.Up)
                        {
                            //walk up
                            newPosition.Y -= movement.MaxSpeed * deltaTimeSeconds;
                            movement.State = MovementState.Walking;
                            movement.Orientation = Orientation.Up;

                        }
                        else if (newDirection == (int)Directions.Down)
                        {
                            // walk down 
                            newPosition.Y += movement.MaxSpeed * deltaTimeSeconds;
                            movement.State = MovementState.Walking;
                            movement.Orientation = Orientation.Down;
                        }
                        else if (newDirection == (int)Directions.Left)
                        {
                            //walk left
                            newPosition.X -= movement.MaxSpeed * deltaTimeSeconds;
                            movement.State = MovementState.Walking;
                            movement.Orientation = Orientation.Left;
                        }
                        else if (newDirection == (int)Directions.Right)
                        {
                            //walk right 
                            newPosition.X += movement.MaxSpeed * deltaTimeSeconds;
                            movement.State = MovementState.Walking;
                            movement.Orientation = Orientation.Left;
                        }
                        else
                        {
                            Console.WriteLine("AI: Error: Unknown direction " + newDirection);
                            movement.State = MovementState.Walking;
                        }
                    }
                } else if(AnubisBehaviour.TailPlayers == this.AnubisBehaviour)
                {
                    //check if tailed player is trapped
                    if(this.tailingPlayer && this.tailedPlayer.GetComponent<Movement>().IsTrapped())
                    {
                        //invalidate tailing
                        this.tailingPlayer = false;
                    }
                    
                    
                    //find closest player alive to follow if no target
                    if(!this.tailingPlayer)
                    {
                        //Console.WriteLine("try to find new player to tail");
                        updateClosestPlayer(ai, transform.Position);
                    }

                    //update direction of anubis to tailing player
                    if(!this.tailingPlayer)
                    {
                        //throw new ArgumentException();
                        //Console.WriteLine("AI: ");
                        return;
                    }

                    Vector2 positionAnubis = transform.Position;
                    int anubis_node_id = movementGraph.ToNodeID(positionAnubis);

                    Vector2 positionPlayer = this.tailedPlayer.GetComponent<Transform>().Position;
                    int tailed_player_node_id = movementGraph.ToNodeID(positionPlayer);

                    Vector2 direction = this.getDirection(ai, positionAnubis, tailedPlayer.GetComponent<Transform>().Position);
                    //Console.WriteLine("anubis, player, direction: " + positionAnubis + ", " + tailedPlayer.GetComponent<Transform>().Position + " -> " + direction);
                    newPosition += direction;

                }
                else
                {
                    // not used currently, is buggy, prob bullshit
                    //
                    //
                    Console.WriteLine("AI: do not use this AI mode");

                    /*
                    int closest_player_dist = default;
                    if(AnubisBehaviour.TailPlayers == this.AnubisBehaviour)
                    {
                        //always tail the closest player
                        closest_player_dist = 999999;
                    } else
                    {
                        closest_player_dist = MaxTailDistance + 1;
                    }

                    Vector2 positionAnubis = transform.Position;
                    int anubis_node_id = movementGraph.ToNodeID(transform.Position);
                    printState(ai);

                    //int anubis_node_id = 5;

                    //Console.WriteLine("using real anubis AI");
                    newPosition = transform.Position;
                    if (!movement.IsTrapped())
                    {
                        //loop over all players and get closest player if he is close enough (<=MaxTailDistance)
                        
                        bool tailed_a_player = false;
                        int tailing_player_nr = -1;
                        Character tailed_player = default;
                        //Console.WriteLine("distance")

                        foreach (Character player in characters)
                        {
                            Transform cur_player_transform = player.GetComponent<Transform>();
                            int nodeIdPlayer = movementGraph.ToNodeID(cur_player_transform.Position);

                            //check if mapping works
                            //Vector2 cur_position = cur_player_transform.Position;
                            
                            //Point tileCoordinates = movementGraph.ToTileCoordinate(nodeIdPlayer);
                            //Vector2 mapped_position = movementGraph.ToPosition(nodeIdPlayer);

                            //Console.WriteLine("chekc if position (re)mapping works. player: " + player.GetComponent<Player>().PlayerID + " positions: " + cur_position + " : " + tileCoordinates + " -> " + mapped_position);
                            //int temp = movementGraph.world_position_to_node_id2(cur_player_transform.ToWorld().Position);
                            //return;

                            //Console.WriteLine("test, player id: " + player.Id + ", position: "+ cur_player_transform.Position.X +"," + cur_player_transform.Position.Y);
                            if (!player.GetComponent<Movement>().IsTrapped() && ai.MovementGraph.PathExists(anubis_node_id, nodeIdPlayer))
                            {

                                int dist = ai.MovementGraph.GetDistance(anubis_node_id, nodeIdPlayer);
                                if (nodeIdPlayer >= 0 && dist >= 0 && dist < closest_player_dist)
                                {
                                    tailed_a_player = true;
                                    closest_player_dist = dist;
                                    tailing_player_nr = player.GetComponent<Player>().PlayerID;
                                    
                                }
                            }
                        }

                        if (tailed_a_player)
                        {
                            Console.WriteLine("Anubis tailes player " + tailing_player_nr);
                            int tailed_player_node_id = movementGraph.ToNodeID(tailed_player.GetComponent<Transform>().Position);

                            if(ai.MovementGraph.PathExists(anubis_node_id, tailed_player_node_id))
                            {

                                Vector2 direction = this.playerDirection(ai, positionAnubis, tailed_player.GetComponent<Transform>().Position);
                                Console.WriteLine("anubis, player, direction: " + positionAnubis + ", " + tailed_player.GetComponent<Transform>().Position + " -> " + direction);
                                newPosition += direction;
                            }
                        }
                        
                    }
                    */
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

        public void Update2(GameTime deltaTime)
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


            }
        }

        private Point toPoint(Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }
    }
}
