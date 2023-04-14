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

        //variables for random movement
        public int NumStepsToGo { get; set; } = -1;
        public Vector2 RandomDirection { get; set; } = default;

        
        //variables to tail players
        private int MaxTailDistance { get; set; } = 10;
        public bool tailingPlayer { get; set; } = false;
        public int tailedPlayerId { get; set; } = default;
        Character tailedPlayer { get; set; } = default;

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

        /// <summary>
        /// Method <c>detailAPlayer</c> removes Anubis of tailing the specified player.
        /// </summary>
        public void detailAPlayer(int playerId)
        {
            if(this.tailedPlayerId == playerId)
            {
                this.tailingPlayer = false;
            }
        }

        /// <summary>
        /// Method <c>detailAllPlayers</c> resets the tailing.
        /// </summary>
        public void detailAllPlayers()
        {
            this.tailingPlayer = false;
        }

        /// <summary>
        /// Method <c>getRandomDirection</c> returns a vector with random orientation of length 1.
        /// </summary>
        private Vector2 getRandomDirection()
        {
            Random rnd = new Random();
            double phi = rnd.NextDouble() * 2*Math.PI;
            return new Vector2((float)Math.Cos(phi), (float)-Math.Sin(phi));
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
                    //check #setps > 0, if not, make a step and decrease by 1
                    // else get new direction, set #steps
                    if (this.NumStepsToGo < 1)
                    {
                        //set new direction & #steps
                        Vector2 newDirection = getRandomDirection();
                        this.RandomDirection = newDirection;
                        //this.RandomDirection = newDirection + this.RandomDirection;
                        //this.RandomDirection.Normalize();
                        this.NumStepsToGo = rnd.Next(150)+30;
                        //Console.WriteLine("set new randomdirection: " + this.RandomDirection + ", #steps: " + this.NumStepsToGo);

                    }

                    newPosition += this.RandomDirection * movement.MaxSpeed;
                    this.NumStepsToGo--;

                } else if(this.AnubisBehaviour == AnubisBehaviour.TailPlayers)
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
                    newPosition += direction * movement.MaxSpeed;

                }
                else
                {
                    Console.WriteLine("AI: this AI mode is not implemented yet");

                }
                //if (currentActions.Contains(PlayerActions.UseObject))
                //{
                //    //check which objects are currently colliding with the player. then check that they are in the orientation the player is looking in
                //    // if the targeted object is iteractable (i.e. an artefact or an item dispenser, or anything else that can be interacted with), trigger the corresponding interaction

                //    //if item dispenser: give player an item of the type corresponding to the dispenser, assuming the player can carry such an item

                //    //if a button: trigger the interaction corresponding to that button

                //    //if a player: check if trapped/unconscious, then check if the current player can free/resurrect that player

                //}

                //set new Anubis Position
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
