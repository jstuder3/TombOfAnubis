using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using QuikGraph;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
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
        public World World { get; set; }

        public AnubisBehaviour AnubisBehaviour { get; set; }
        public AISystem(World world, AnubisBehaviour anubisBehaviour)
        {
            World = world;
            AnubisBehaviour = anubisBehaviour;
        }

        protected enum Directions
        {
            Up,
            Right,
            Down,
            Left
        }

        private Random rnd = new Random();


        //variables for random movement        
        public int NumStepsToGo { get; set; } = -1;
        public Vector2 RandomDirection { get; set; } = default;

        
        //variables to tail players
        private int MaxTailDistance { get; set; } = 4;
        private int DetailDistance { get; set; } = 7;
        public bool tailingPlayer { get; set; } = false;
        public int tailedPlayerId { get; set; } = default;
        Character tailedPlayer { get; set; } = default;

        //Rage Level System
        //public int rageLevel = 0;
        private bool rageMode = false;
        private int nArtefactsCollected = 0;


        //True AI logic:
        private int randomTileNr = default;
        private bool randomTileNrSet = false;


        //avoidWallSystem:
        public bool wallBottomLeft = false;
        public bool wallBottomRight = false;
        public bool wallTopLeft = false;
        public bool wallTopRight = false;

        public void printState(AI ai)
        {
            Entity entity = ai.Entity;
            //Transform transform = entity.GetComponent<Transform>();
            List<Character> characters = World.GetChildrenOfType<Character>();

            //Anubis:
            Vector2 positionAnubis = entity.TopLeftCornerPosition();
            int nodeIdAnubis = ai.MovementGraph.ToNodeID(positionAnubis);

            Debug.WriteLine("----Print AI System State:------");
            Debug.WriteLine("Anubis: position, nodeId: " + positionAnubis + ", " + nodeIdAnubis);

            foreach (Character player in characters)
            {
                //Transform transformPlayer = player.GetComponent<Transform>();
                Vector2 positionPlayer = player.TopLeftCornerPosition();
                int nodeIdPlayer = ai.MovementGraph.ToNodeID(positionPlayer);
                Debug.WriteLine("Player nr: " + player.GetComponent<Player>().PlayerID + ", position " + positionPlayer + ", nodeId " + nodeIdPlayer + ", distance: " + ai.MovementGraph.GetDistance(nodeIdAnubis, nodeIdPlayer));
            }


        }

        public bool rageModeActivated()
        {
            return this.rageMode;
        }

        public void activateRageMode()
        {
            
            if(rageModeActivated())
            {
                //nothing to do. ragemode already activated
                return;
            }
            
            AI ai = GetComponents().First();
            Entity entity = ai.Entity;
            Movement movement = entity.GetComponent<Movement>();


            //increase maxspeed:
            entity.AddComponent(new GameplayEffect(EffectType.AdditiveSpeedModification, 0f, 50f, Visibility.Both));
            this.MaxTailDistance += 4;
            this.DetailDistance += 2;

            this.rageMode = true;
            Debug.WriteLine("AI: RRRRRRagemode activated");

            //change anubis particles to red
            ParticleEmitter emitter = entity.GetComponent<ParticleEmitter>();
            ParticleEmitterConfiguration pec = emitter.EmitterConfiguration;
            pec.RandomizedTintMin = Color.DarkRed;
            pec.RandomizedTintMax = Color.DarkGray;
            emitter.EndEmitter();
            entity.AddComponent(new ParticleEmitter(pec));

        }

        public void triggerRageModeProbability(bool collectedNewArtefact)
        {
            int nPlayers = World.GetChildrenOfType<Character>().Count();
            if (collectedNewArtefact)
            {
                this.nArtefactsCollected += Math.Min(nPlayers, this.nArtefactsCollected+1);
            }
            double threshold = (double)this.nArtefactsCollected / nPlayers;
            double rand = this.rnd.NextDouble();
            if (rand <= threshold)
            {
                this.activateRageMode();
                AudioController.PlaySong("gameFastTrack");
            }
        }



        bool wallDetected()
        {
            return this.wallTopRight || this.wallTopLeft || this.wallBottomRight || this.wallBottomLeft;
        }

        Vector2 wallDirectionAugmentation(Vector2 direction)
        {
            
            //adapt direction if walls are close
            bool wallRight = this.wallTopRight && this.wallBottomRight;
            bool wallLeft = this.wallTopLeft && this.wallBottomLeft;
            bool wallBottom = this.wallBottomLeft && this.wallBottomRight;
            bool wallTop = this.wallTopLeft && this.wallTopRight;

            if (wallTop)
            {
                direction.Y = Math.Max(0, direction.Y);
            }
            else if (wallBottom)
            {
                direction.Y = Math.Min(0, direction.Y);
            }
            else if (wallLeft)
            {
                direction.X = Math.Max(0, direction.X);
            }
            else if (wallRight)
            {
                direction.X = Math.Min(0, direction.X);
            }
            else if (this.wallTopLeft)
            {
                //cannot move to the left&top
                if (direction.X < direction.Y)
                {
                    //more to the left than top, thus wall to the left, walk down until no wall
                    direction = new Vector2(0, 1);
                }
                else
                {
                    //more to the top than left, thus wall to the top, walk right until no wall
                    direction = new Vector2(1, 0);
                }
            }
            else if (this.wallTopRight)
            {
                if (direction.X > -1 * direction.Y)
                {
                    //more to the right than top, thus wall to the right, walk down until no wall
                    direction = new Vector2(0, 1);
                }
                else
                {
                    //more to the top than right, thus wall to the top, walk left until no wall
                    direction = new Vector2(-1, 0);
                }
            }
            else if (this.wallBottomLeft)
            {
                if (-1 * direction.X > direction.Y)
                {
                    //more to the left than bottom, thus wall to the left, walk up until no wall
                    direction = new Vector2(0, -1);
                }
                else
                {
                    //more to the bottom than left, thus wall to the bottom, walk right until no wall
                    direction = new Vector2(1, 0);
                }
            }
            else if (this.wallBottomRight)
            {
                if (direction.X > direction.Y)
                {
                    //more to the right than bottom, thus wall to the right, walk up until no wall
                    direction = new Vector2(0, -1);
                }
                else
                {
                    //more to the bottom than right, thus wall to the bottom, walk left until no wall
                    direction = new Vector2(-1, 0);
                }
            }
            return direction;
        }

        private Vector2 getDirection2(AI ai, Vector2 anubisPosition, Vector2 playerPosition)
        {
            //asumes this.TailingPlayer is true

            int nodeIdAnubis = ai.MovementGraph.ToNodeID(anubisPosition);
            int nodeIdPlayer = ai.MovementGraph.ToNodeID(playerPosition);

            Point tileCoordinateAnubis = ai.MovementGraph.ToTileCoordinate(nodeIdAnubis);
            Point tileCoordinatePlayer = ai.MovementGraph.ToTileCoordinate(nodeIdPlayer);

            Vector2 direction = new Vector2(0, 0);


            if (nodeIdAnubis == nodeIdPlayer || ai.MovementGraph.isTileNeighbor(tileCoordinateAnubis, tileCoordinatePlayer))
            {
                //directly walk towards the player 
                //Debug.WriteLine("Anubis walks directly to Player");
                direction = playerPosition - anubisPosition;
            } else
            {
                //walk along the path of tile nodes
                //Debug.WriteLine("Anubis walks towards Nodes");
                if (!ai.MovementGraph.PathExists(nodeIdAnubis, nodeIdPlayer))
                {
                    Debug.WriteLine("AI: Error: Anbuis cound not find a Path to cur tailed player. Using RandomDirection");
                    this.tailingPlayer = false;
                    //use random movement
                    direction = getRandomDirection();
                } else
                {
                    int smoothness = 3;
                    if (wallDetected()) { smoothness = 2; } //needed to correctly augment direction
                    
                    
                    IEnumerable<Edge<int>> targets = ai.MovementGraph.GetNextNEdges(nodeIdAnubis, nodeIdPlayer, smoothness);
                    //take mean of all targets
                    int weight = 0;
                    foreach(Edge<int> target in targets)
                    {
                        Vector2 edgeDirection = (ai.MovementGraph.ToPosition(target.Target) - ai.MovementGraph.ToPosition(target.Source));
                        edgeDirection.Normalize();
                        direction += edgeDirection / (float)Math.Pow(2,weight++);
                    }
                    direction.Normalize();
                }

            }
            //normalization after raw direction claculation
            direction.Normalize();


            //print stuff
            bool print_stuff = false;
            if (print_stuff && (this.wallTopLeft || this.wallTopRight || this.wallBottomRight || this.wallBottomLeft))
            {
                Debug.WriteLine("direction augmentation, direction before: " + direction);
                if (this.wallTopLeft) { Debug.WriteLine("Wall TopLeft"); };
                if (this.wallTopRight) { Debug.WriteLine("Wall TopRight"); };
                if (this.wallBottomRight) { Debug.WriteLine("Wall BottomRight"); };
                if (this.wallBottomLeft) { Debug.WriteLine("Wall BottomLeft"); };
            }


            
            direction = wallDirectionAugmentation(direction) ;

            direction.Normalize();
            if (this.wallTopLeft || this.wallTopRight || this.wallBottomRight || this.wallBottomLeft)
            {
                //Debug.WriteLine("diretion after: " + direction);
                //Debug.WriteLine("direction augmentation");
            }
            else
            {
                //Debug.WriteLine("no wall direction augmentation, direction: " + direction);
            }
            return direction;
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
                //Debug.WriteLine("State: direcToPlayer, anubis, player, target, direction, direction norml.: " + anubisPosition + ", " + playerPosition + ", " + playerPosition + ", " + temp2 + ", " + difference);
                return difference;
            }


            //else use direction to next tile\node of the path to the tailed player

            if (!ai.MovementGraph.PathExists(nodeIdAnubis, nodeIdPlayer))
            {
                Debug.WriteLine("Error: Anbuis cound not find a Path to cur tailed player");

                //hotfix:
                this.tailingPlayer = false;
                return new Vector2(0, 0);
            }


            Vector2 target = ai.MovementGraph.GetTargetToWalkTo(nodeIdAnubis, nodeIdPlayer);
            Vector2 target2 = ai.MovementGraph.getNthTargetToWalkTo(nodeIdAnubis, nodeIdPlayer, 2);
            if(target2.LengthSquared() != 0)
            {
                target = (float)0.5 * (target + target2);
            }

            Vector2 direction = target - anubisPosition;
            
            //somehow sometimes anubis position is already at target position??? try hotfix
            if(direction.LengthSquared() < 1000)
            {
                //Debug.WriteLine("AI: movement HotFix needed, tile distance too small");
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
            //Debug.WriteLine("State: direcToTile, anubis, player, target, direction, direciton norml.: " + anubisPosition + ", " + playerPosition + ", " + target + ", " + temp + ", " + direction);
            //Debug.WriteLine("direction length: " + temp.LengthSquared());
            bool print_stuff = false;
            if (print_stuff && (this.wallTopLeft || this.wallTopRight || this.wallBottomRight || this.wallBottomLeft))
            {
                Debug.WriteLine("direction augmentation, direction before: " + direction);
                if (this.wallTopLeft) { Debug.WriteLine("Wall TopLeft"); };
                if (this.wallTopRight) { Debug.WriteLine("Wall TopRight"); };
                if (this.wallBottomRight) { Debug.WriteLine("Wall BottomRight"); };
                if (this.wallBottomLeft) { Debug.WriteLine("Wall BottomLeft"); };
            }
            //adapt direction if walls are close
            bool wallRight = this.wallTopRight && this.wallBottomRight;
            bool wallLeft = this.wallTopLeft && this.wallBottomLeft;
            bool wallBottom = this.wallBottomLeft && this.wallBottomRight;
            bool wallTop = this.wallTopLeft && this.wallTopRight;
            
            if (wallTop)
            {
                direction.Y = Math.Max(0, direction.Y);
            } 
            else if (wallBottom)
            {
                direction.Y = Math.Min(0, direction.Y);
            } 
            else if (wallLeft)
            {
                direction.X = Math.Max(0, direction.X);
            }
            else if (wallRight)
            {
                direction.X = Math.Min(0, direction.X);
            }
            else if (this.wallTopLeft)
            {
                //cannot move to the left&top
                if (direction.X < direction.Y)
                {
                    //more to the left than top, thus wall to the left, walk down until no wall
                    direction = new Vector2(0,1);
                }
                else
                {
                    //more to the top than left, thus wall to the top, walk right until no wall
                    direction = new Vector2(1,0);
                }
            }
            else if (this.wallTopRight)
            {
                if (direction.X > -1 * direction.Y)
                {
                    //more to the right than top, thus wall to the right, walk down until no wall
                    direction = new Vector2(0,1);
                }
                else
                {
                    //more to the top than right, thus wall to the top, walk left until no wall
                    direction = new Vector2(-1,0);
                }
            }
            else if (this.wallBottomLeft)
            {
                if (-1*direction.X > direction.Y)
                {
                    //more to the left than bottom, thus wall to the left, walk up until no wall
                    direction = new Vector2(0,-1);
                } else
                {
                    //more to the bottom than left, thus wall to the bottom, walk right until no wall
                    direction = new Vector2(1,0);
                }
            } else if (this.wallBottomRight)
            {
                if(direction.X > direction.Y)
                {
                    //more to the right than bottom, thus wall to the right, walk up until no wall
                    direction = new Vector2(0,-1);
                } else
                {
                    //more to the bottom than right, thus wall to the bottom, walk left until no wall
                    direction = new Vector2(-1,0);
                }
            }





            direction.Normalize();
            if (this.wallTopLeft || this.wallTopRight || this.wallBottomRight || this.wallBottomLeft)
            {
                Debug.WriteLine("diretion after: " + direction);
            } else
            {
                //Debug.WriteLine("no wall direction augmentation");
            }
            return direction;
        }

        public int GetDistToclosestPlayer(AI ai, Vector2 positionAnubis)
        {
            MovementGraph movementGraph = ai.MovementGraph;
            List<Character> characters = World.GetChildrenOfType<Character>();
            int nodeIdAnubis = movementGraph.ToNodeID(positionAnubis);
            int minDist = 999999;

            foreach (Character player in characters)
            {
                if (!player.GetComponent<Movement>().IsVisibleToAnubis())
                {
                    continue;
                }
                int nodeIdPlayer = movementGraph.ToNodeID(player.CenterPosition());
                int dist = movementGraph.GetDistance(nodeIdAnubis, nodeIdPlayer);
                //Debug.WriteLine("dist to current player: id: " + nodeIdPlayer + ", dist: " + dist);
                if (dist >= 0 && dist < minDist)
                {
                    minDist = dist;
                }
            }
            return minDist;
        }

        public Tuple<bool,Character> GetClosestPlayer(AI ai, Vector2 anubisPosition)
        {
            MovementGraph movementGraph = ai.MovementGraph;
            List<Character> characters = World.GetChildrenOfType<Character>();

            int nodeIdAnubis = movementGraph.ToNodeID(anubisPosition);
            int closestPlayerDist = 999999;
            bool foundAPlayer = false;
            Character closestPlayer = default;

            foreach (Character player in characters)
            {
                //Transform cur_player_transform = player.GetComponent<Transform>();
                int nodeIdPlayer = movementGraph.ToNodeID(player.CenterPosition());
                //Debug.WriteLine("player position: " + player.CenterPosition());

                if (player.GetComponent<Movement>().IsVisibleToAnubis() && !player.GetComponent<Movement>().IsTrapped() && ai.MovementGraph.PathExists(nodeIdAnubis, nodeIdPlayer))
                {

                    int dist = ai.MovementGraph.GetDistance(nodeIdAnubis, nodeIdPlayer);
                    if (nodeIdPlayer >= 0 && dist >= 0 && dist < closestPlayerDist)
                    {
                        foundAPlayer = true;
                        closestPlayerDist = dist;
                        closestPlayer = player;
                    }
                }
            }
            return Tuple.Create(foundAPlayer, closestPlayer);
        }

        public bool updateClosestPlayer(AI ai, Vector2 anubisPosition)
        {
            //Debug.WriteLine("cur no tailed player, try to change now");
            MovementGraph movementGraph = ai.MovementGraph;
            List<Character> characters = World.GetChildrenOfType<Character>();


            int nodeIdAnubis = movementGraph.ToNodeID(anubisPosition);

            int closest_player_dist = 999999;
            bool tailed_a_player = false;
            int tailing_player_nr = -1;
            Character tailed_player = default;

            foreach (Character player in characters)
            {
                //Transform cur_player_transform = player.GetComponent<Transform>();
                int nodeIdPlayer = movementGraph.ToNodeID(player.CenterPosition());
                //Debug.WriteLine("player position: " + player.CenterPosition());

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
                Debug.WriteLine("AI: tailing new player: " + this.tailedPlayer.GetComponent<Player>().PlayerID);
                return true;
            }
            Debug.WriteLine("AI: udpateClosestPlayer failed: could not find any reachable player");
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

        bool validPosition(Vector2 pos)
        {
            //prob not the best way to do this
            Point tileCoordinates = Session.GetInstance().Map.PositionToTileCoordinate(pos);
            bool ret = Session.GetInstance().Map.GetCollisionLayerValue(tileCoordinates) == 0;
            //Debug.WriteLine("vPos: " + tileCoordinates + ", tileValue: " + Session.GetInstance().Map.GetCollisionLayerValue(tileCoordinates) + ", ret: " + ret + ", neg: " + !ret);
            return ret;
        }

        void updateWallSystem(Vector2 pos, Vector2 diagVec, float offset, int verbose)
        {
            //Debug.WriteLine("length test: " + new Vector2((float)Math.Cos(0.5 * Math.PI), (float)-Math.Sin(0.5 * Math.PI)).LengthSquared());
            float increaser = (float)1.075;
            float decreaser = (float)1;
            Vector2 topIncreaser = new Vector2(0, -10);

            Vector2 topLeft = pos + increaser*diagVec + topIncreaser;
            Vector2 topRight = pos + increaser*new Vector2(-1 * diagVec.X, diagVec.Y) + topIncreaser;
            Vector2 bottomLeft = pos + increaser*new Vector2(diagVec.X, -1 * diagVec.Y);
            Vector2 bottomRight = pos - increaser*diagVec;

            

            //update all booleans
            this.wallBottomLeft = !this.validPosition(bottomLeft);
            this.wallBottomRight = !this.validPosition(bottomRight);
            this.wallTopLeft = !this.validPosition(topLeft);
            this.wallTopRight = !this.validPosition(topRight);

            if (verbose >= 1)
            {
                if (this.wallTopLeft) { Debug.WriteLine("Wall TopLeft"); };
                if (this.wallTopRight) { Debug.WriteLine("Wall TopRight"); };
                if (this.wallBottomRight) { Debug.WriteLine("Wall BottomRight"); };
                if (this.wallBottomLeft) { Debug.WriteLine("Wall BottomLeft"); };

            }

        }

        public override void Update(GameTime deltaTime)
        {

            Random rnd = new Random();
            

            foreach (AI ai in GetComponents())
            {
                //spawn anubis once on a fixed position
                bool did_spawn = false;
                if (!did_spawn)
                {
                    Transform transformTemp = ai.Entity.GetComponent<Transform>();
                    //TransformTemp = 

                    did_spawn = true;
                }
                
                
                //Debug.WriteLine("reached anubis section");
                Entity entity = ai.Entity;
                Transform transform = entity.GetComponent<Transform>();
                Movement movement = entity.GetComponent<Movement>();

                //Debug.WriteLine("Anubis vliad position: " + Session.GetInstance().Map.ValidTileCoordinates(Session.GetInstance().Map.PositionToTileCoordinate(entity.CenterPosition())));
                //Debug.WriteLine("anubis position: " + entity.CenterPosition());
                //only update if the entity is capable of moving (i.e. not dead, not trapped and not stunned)
                if (!movement.CanMove()) continue;

                MovementGraph movementGraph = ai.MovementGraph;
                List<Character> characters = World.GetChildrenOfType<Character>();
                List<Artefact> artefacts = World.GetChildrenOfType<Artefact>();

                RectangleCollider collider = entity.GetComponent<RectangleCollider>();

                // Node anubis = movementGraph.GetNode(entity)
                // Node c1 = movementGraph.GetNode(characters[0])
                // List<Vector2> currentPath = movementGraph.CreatePath(anubis, c1)
                // Vector2 currentTarget = currentPath[0]

                // movement.IsWalking = false;
                float deltaTimeSeconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;
                //Vector2 newPosition = transform.Position;
                Vector2 newPosition = entity.CenterPosition();

                

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
                        //Debug.WriteLine("set new randomdirection: " + this.RandomDirection + ", #steps: " + this.NumStepsToGo);

                    }

                    newPosition += this.RandomDirection * movement.MaxSpeed * deltaTimeSeconds;
                    this.NumStepsToGo--;

                } else if(this.AnubisBehaviour == AnubisBehaviour.TailPlayers)
                {
                    
                    //check if tailed player is trapped or invisible
                    if(this.tailingPlayer && !this.tailedPlayer.GetComponent<Movement>().IsVisibleToAnubis())
                    {
                        //invalidate tailing
                        this.tailingPlayer = false;
                    }
                    
                    
                    //find closest player alive to follow if no target
                    if(!this.tailingPlayer)
                    {
                        //Debug.WriteLine("try to find new player to tail");
                        updateClosestPlayer(ai, entity.TopLeftCornerPosition());
                    }

                    //update direction of anubis to tailing player
                    if(!this.tailingPlayer)
                    {
                        //throw new ArgumentException();
                        //Debug.WriteLine("AI: ");
                        return;
                    }
                    
                    Vector2 positionAnubis = entity.CenterPosition();
                    float offset = (float)1.1 * (entity.CenterPosition() - entity.TopLeftCornerPosition()).Length();
                    updateWallSystem(positionAnubis, entity.TopLeftCornerPosition() - entity.CenterPosition(), offset, 0);


                    Vector2 positionPlayer = tailedPlayer.CenterPosition();
                    //int tailed_player_node_id = movementGraph.ToNodeID(positionPlayer);

                    Vector2 direction;
                    //direction = this.getDirection(ai, positionAnubis, tailedPlayer.CenterPosition());
                    Vector2 direction2 = this.getDirection2(ai, positionAnubis, tailedPlayer.CenterPosition());
                    //Debug.WriteLine("directions: " + direction + ", --- " + direction2);
                    direction = direction2;



                    //Debug.WriteLine("new Position: " + newPosition);

                    //Debug.WriteLine("Anubis movement direction: " + direction);

                    //Debug.WriteLine("anubis, player, direction: " + positionAnubis + ", " + tailedPlayer.GetComponent<Transform>().Position + " -> " + direction);
                    newPosition += direction * movement.MaxSpeed * deltaTimeSeconds;
                    //newPosition = newPosition;

                    
                } else if (this.AnubisBehaviour == AnubisBehaviour.TrueAI)
                {
                    //moves randomly until a player too close
                    //random movement: determend movement to a random place in the labyrinth

                    //initialization stuff
                    Vector2 positionAnubis = entity.CenterPosition();


                    //first updated trueAI system (tailing player, check distances usw)
                    //first check if a valid tailing 
                    if(this.tailingPlayer)
                    {
                        Vector2 positionTailedPlayer = this.tailedPlayer.CenterPosition();
                        int nodeIdTailedPlayer = movementGraph.ToNodeID(positionTailedPlayer);
                        int nodeIdAnubis = movementGraph.ToNodeID(positionAnubis);

                        if(!this.tailedPlayer.GetComponent<Movement>().IsVisibleToAnubis() || movementGraph.GetDistance(nodeIdAnubis, nodeIdTailedPlayer) >= this.DetailDistance)
                        {
                            this.tailingPlayer = false;
                            Debug.WriteLine("AI: tailed Player not visible or too far, detailed");
                        } else
                        {
                            //check if other player is closer
                            int tailingPlayerDist = movementGraph.GetDistance(nodeIdAnubis, nodeIdTailedPlayer);
                            int distToClosestPlayer = GetDistToclosestPlayer(ai, positionAnubis);
                            if(distToClosestPlayer < tailingPlayerDist)
                            {
                                Tuple<bool,Character> tryClosestPlayer = GetClosestPlayer(ai, positionAnubis);
                                if (tryClosestPlayer.Item1)
                                {
                                    this.tailedPlayer = tryClosestPlayer.Item2;
                                    Debug.WriteLine("AI: switched tailing to closer Player, tailing: " + this.tailedPlayer.GetComponent<Player>().PlayerID);
                                }
                                
                            }
                        }
                    }

                    if(!this.tailingPlayer) {
                        //check if a visible palyer is close enough
                        int distToClosestPlayer = GetDistToclosestPlayer(ai, positionAnubis);
                        if (distToClosestPlayer <= this.MaxTailDistance)
                        {
                            Tuple<bool, Character> tryGetClosestPlayer = GetClosestPlayer(ai, positionAnubis);
                            this.tailingPlayer = tryGetClosestPlayer.Item1;
                            if (tryGetClosestPlayer.Item1)
                            {
                                this.tailedPlayer = tryGetClosestPlayer.Item2;
                                Debug.WriteLine("AI: found player, tailing: " + this.tailedPlayer.GetComponent<Player>().PlayerID);
                            }
                            
                        }


                    }

                    if (this.tailingPlayer)
                    {
                        Vector2 positionTailedPlayer = this.tailedPlayer.CenterPosition();
                        int nodeIdTailedPlayer = movementGraph.ToNodeID(positionTailedPlayer);
                        int nodeIdAnubis = movementGraph.ToNodeID(positionAnubis);

                        float offset = (float)1.1 * (entity.CenterPosition() - entity.TopLeftCornerPosition()).Length();
                        updateWallSystem(positionAnubis, entity.TopLeftCornerPosition() - entity.CenterPosition(), offset, 0);

                        Vector2 direction = this.getDirection2(ai, positionAnubis, positionTailedPlayer);
                        newPosition += direction * movement.MaxSpeed * deltaTimeSeconds;

                    } else
                    {
                        //walk to a random tile
                        //first check if at the tile
                        if (!this.randomTileNrSet || movementGraph.atSameTile(this.randomTileNr, positionAnubis))
                        {
                            //choose new tile to walk to
                            this.randomTileNr = movementGraph.GetRandomTileNr(movementGraph.ToNodeID(positionAnubis));
                            this.randomTileNrSet = true;
                        }

                        //update wall system befor e direction fkt called
                        float offset = (float)1.1 * (entity.CenterPosition() - entity.TopLeftCornerPosition()).Length();
                        updateWallSystem(positionAnubis, entity.TopLeftCornerPosition() - entity.CenterPosition(), offset, 0);
                        //now valid tile nr as a goal is set, walk towards it
                        Vector2 direction = this.getDirection2(ai, positionAnubis, movementGraph.ToCenterTilePosition(this.randomTileNr));

                        newPosition += direction * movement.MaxSpeed * deltaTimeSeconds;

                    }
                }
                else
                {
                    Debug.WriteLine("AI: unknown AI mode");

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
                Vector2 shift = entity.TopLeftCornerPosition() - entity.CenterPosition();
                transform.Position = newPosition + shift;

                return;

                
            }
        }

        private Point toPoint(Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }
    }
}
