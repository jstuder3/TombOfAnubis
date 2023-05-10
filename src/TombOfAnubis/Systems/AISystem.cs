using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using QuikGraph;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
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

        public AnubisBehaviour AnubisBehaviour { get; set; }
        public AISystem(AnubisBehaviour anubisBehaviour)
        {
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

        //variables to increase ms when tailed same player over a longer time
        private int increaseMsOverTimeDistance { get; set; } = 3;
        public bool msOverTimeActive = false;

        public int increaseMsoverTimeAccumulation = 0;
        public float timeAccumulatorMiliSec = 0;

        //Event System
        //public int rageLevel = 0;
        private bool rageMode = false;
        public ParticleEmitter rageModeParticlesEmitter = default;

        //variables for Castmode
        private bool cmInitSucceeded = false;
        private Vector2 cmChosenPos = default;

        //variables for blockPowerUpsEvent
        public bool powerupsBlockedEvent = false;


        //True AI logic:
        private int randomTileNr = default;
        private bool randomTileNrSet = false;


        //avoidWallSystem:
        public Vector2 wallBottomLeftPos = default;
        public Vector2 wallBottomRightPos = default;
        public Vector2 wallTopLeftPos = default;
        public Vector2 wallTopRightPos = default;

        public bool wallBottomLeft = false;
        public bool wallBottomRight = false;
        public bool wallTopLeft = false;
        public bool wallTopRight = false;

        public int quadrant = 0; //1: top, 2: left, 3: right, 4: bottom

        public void printState(AI ai)
        {
            Entity entity = ai.Entity;
            //Transform transform = entity.GetComponent<Transform>();
            List<Character> characters = Session.GetInstance().World.GetChildrenOfType<Character>();

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

        public void increaseMsCauseTailing(GameTime deltaTime)
        {
            //increase the timeAccMiliSec, if more than 10, add 1ms per 10miliSec.
            float miliSec = (float)deltaTime.ElapsedGameTime.TotalMilliseconds;
            //Debug.WriteLine("elapsed Time[ms]: " + miliSec);
            this.timeAccumulatorMiliSec += miliSec;

            if(this.timeAccumulatorMiliSec > 100)
            {
                AI ai = GetComponents().First();
                Entity entity = ai.Entity;
                int msIncreasage = (int)(this.timeAccumulatorMiliSec / 50.0);

                //nerf tialingMSIncreasage if playing solo
                int nPlayers = Session.GetInstance().World.GetChildrenOfType<Character>().Count();
                if (nPlayers == 1)
                {
                    msIncreasage /= 2;
                }               

                entity.GetComponent<Movement>().BaseMovementSpeed += msIncreasage;
                this.increaseMsoverTimeAccumulation += msIncreasage;
                this.timeAccumulatorMiliSec = this.timeAccumulatorMiliSec % 50;
            }
        }

        public void decreaseIncreasgeOfMscuasetailing()
        {
            if (this.increaseMsoverTimeAccumulation > 0)
            {
                AI ai = GetComponents().First();
                Entity entity = ai.Entity;
                entity.GetComponent<Movement>().BaseMovementSpeed -= this.increaseMsoverTimeAccumulation;
                Debug.WriteLine("AI: remove TailingMS, decreasedMs by: " + this.increaseMsoverTimeAccumulation + ", new Ms: " + entity.GetComponent<Movement>().BaseMovementSpeed);

                this.increaseMsoverTimeAccumulation = 0;
                this.timeAccumulatorMiliSec = 0;

            }
        }

        public void activateBlockPowerups()
        {
            Debug.WriteLine("Event: powerups are deactivated");
            this.powerupsBlockedEvent = true;
        }

        public void deactivateBlockPowerups()
        {
            
            Debug.WriteLine("Event: powerups are activated again");
            this.powerupsBlockedEvent = false;
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

            //increase asnubis stats (ms & tailing):
            this.rageMode = true;
            entity.AddComponent(new GameplayEffect(EffectType.AdditiveSpeedModification, 0f, 100f, Visibility.Both));
            this.MaxTailDistance += 4;
            this.DetailDistance += 2;

            //change anubis particles to red
            
            ParticleEmitter Emitter = entity.GetComponent<ParticleEmitter>();
            ParticleEmitterConfiguration pec = Emitter.EmitterConfiguration;
            pec.RandomizedTintMin = Color.DarkRed;
            pec.RandomizedTintMax = Color.DarkGray;
            //do not pause standart anubis particle effects but temporary spawn red ones with base effect
            //Emitter.EndEmitter(); 
            this.rageModeParticlesEmitter = new ParticleEmitter(pec);
            entity.AddComponent(rageModeParticlesEmitter);
            


            Debug.WriteLine("AI: RRRRRRagemode activated");
        }

        public void deactivateRageMode()
        {
            //Debug.WriteLine("Event: deactiveRagemode, ragemode stat: " + this.rageModeActivated());
            if(!rageModeActivated())
            {
                return;
            }
            AI ai = GetComponents().First();
            Entity entity = ai.Entity;

            //increase maxspeed:
            this.rageMode = false;
            entity.AddComponent(new GameplayEffect(EffectType.AdditiveSpeedModification, 0f, -80f, Visibility.Both));
            this.MaxTailDistance -= 4;
            this.DetailDistance -= 2;
            this.rageModeParticlesEmitter.EndEmitter();

            Debug.WriteLine("AI: Ragemode deactivated");
        }

        public void initiateCastMode()
        {
            
            AI ai = GetComponents().First();
            
            //MovementGraph movementGraph = ai.MovementGraph;
            List<Character> characters = Session.GetInstance().World.GetChildrenOfType<Character>();

            int countNVisible = 0;
            foreach (Character player in characters)
            {
                if (player.GetComponent<Movement>().IsVisibleToAnubis())
                {
                    countNVisible++;
                }
            }
            if(countNVisible > 0)
            {
                this.cmInitSucceeded = true;
                int chosenPlayerNr = this.rnd.Next(countNVisible);
                bool succeededd = false;

                int counter = 0;
                foreach(Character player in characters)
                {
                    if (player.GetComponent<Movement>().IsVisibleToAnubis())
                    {
                        if (counter == chosenPlayerNr)
                        {
                            this.cmChosenPos = player.TopLeftCornerPosition();
                            this.cmInitSucceeded = true;
                            succeededd = true;
                            Debug.WriteLine("Event: CastMode inititated!!! cur anubis pos: " + ai.Entity.GetComponent<Transform>().Position + ", stored tele pos: " + this.cmChosenPos);

                            //add particle effect at teleport target location  (code copied from ragemode)
                            //floating particles
                            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
                            pec.LocalPosition = this.cmChosenPos + new Vector2(25f, 100f);
                            pec.RandomizedSpawnPositionRadius = 50f;
                            //doesn't work yet
                            pec.ParticlesMoveWithEntity = false;
                            pec.Texture = ParticleTextureLibrary.BasicParticle;
                            pec.SpriteLayer = 1;
                            pec.InitialAlpha = 0.5f;
                            pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
                            pec.RandomizedTintMin = Color.DarkRed;
                            pec.RandomizedTintMax = Color.DarkGray;
                            pec.Scale = Vector2.One * 0.4f;
                            pec.ScalingMode = ScalingMode.Constant;
                            pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
                            pec.EmitterDuration = 2f;
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

                            Session.GetInstance().World.AddComponent(new ParticleEmitter(pec));
                            return;
                        } else
                        {
                            counter++;
                        }
                    }
                }
                if (!succeededd)
                {
                    Debug.WriteLine("Event: CastMode initialization failed");
                }
            } 
        }

        public void executeCastMode()
        {
            if(this.cmInitSucceeded)
            {
                
                AI ai = GetComponents().First();
                Entity anubis = ai.Entity;
                Transform transform = anubis.GetComponent<Transform>();

                //set anubis position to the new location
                transform.Position = this.cmChosenPos;

                Debug.WriteLine("Event: CastMode Execution. new anuibs Pos: " + transform.Position);

                //spawn particles at the new location
                //particles that are spawned at the new location to show an "impact"
                ParticleEmitterConfiguration teleport_impact = new ParticleEmitterConfiguration();
                teleport_impact.LocalPosition = cmChosenPos;
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

                //reset flag for next iteration
                this.cmInitSucceeded = false;
            }
        }


        bool horizontalTileNeigh(AI ai, Vector2 pos1, Vector2 pos2)
        {
            int nodeIdPos1 = ai.MovementGraph.ToNodeID(pos1);
            int nodeIdPos2 = ai.MovementGraph.ToNodeID(pos2);

            Vector2 nodeIdPos1Position = ai.MovementGraph.ToPosition(nodeIdPos1);
            Vector2 nodeIdPos2Position = ai.MovementGraph.ToPosition(nodeIdPos2);

            if (nodeIdPos1Position.Y == nodeIdPos2Position.Y)
            {
                //same tiles or horizontal neighbouring tiles
                return true;
            } 
            return false;
        }

        int insideTileLocation(Vector2 pos)
        {
            // 1: top quad, 2: right quad, 3: bottom quad, 4: left quad
            Map map = Session.GetInstance().Map;
            Point tileIndex = map.PositionToTileCoordinate(pos);

            Vector2 tileCentrePos = map.TileCoordinateToTileCenterPosition(tileIndex);
            Vector2 tileTopLeftPos = map.TileCoordinateToPosition(tileIndex);

            Vector2 diag = tileCentrePos - tileTopLeftPos;

            Vector2 tileTopMidPos = tileTopLeftPos + new Vector2(diag.X, 0);
            Vector2 tileLeftMidPos = tileTopLeftPos + new Vector2(0, diag.Y);
            Vector2 tileBottomMidPos = tileLeftMidPos + diag;
            Vector2 tileRightMidPos = tileTopMidPos + diag;

            double distToTop = (pos - tileTopMidPos).LengthSquared();
            double distToLeft = (pos - tileLeftMidPos).LengthSquared();
            double distToBottom = (pos - tileBottomMidPos).LengthSquared();
            double distToRight = (pos - tileRightMidPos).LengthSquared();


            double minDist = Math.Min(Math.Min(distToTop, distToLeft), Math.Min(distToRight, distToBottom));
            int casee = 0;
            if (minDist == distToTop)
            {
                casee = 1;
            } else if (minDist == distToRight)
            {
                casee = 2;
            } else if (minDist == distToBottom)
            {
                casee = 3;
            } else if (minDist == distToLeft)
            {
                casee = 4;
            }
            return casee;
        }


        bool verticalTileNeigh(AI ai, Vector2 pos1, Vector2 pos2)
        {
            int nodeIdPos1 = ai.MovementGraph.ToNodeID(pos1);
            int nodeIdPos2 = ai.MovementGraph.ToNodeID(pos2);

            Vector2 nodeIdPos1Position = ai.MovementGraph.ToPosition(nodeIdPos1);
            Vector2 nodeIdPos2Position = ai.MovementGraph.ToPosition(nodeIdPos2);

            if (nodeIdPos1Position.X == nodeIdPos2Position.X)
            {
                //same tiles or horizontal neighbouring tiles
                return true;
            }
            return false;
        }



        bool wallDetected()
        {
            return this.wallTopRight || this.wallTopLeft || this.wallBottomRight || this.wallBottomLeft;
        }

        Vector2 wallDirectionAugmentation3(AI ai, Vector2 direction)
        {
            direction.Normalize();
            Vector2 direction_before = direction;
            //Debug.WriteLine("direction initially: " + direction);
            Entity anubis = ai.Entity;
            Map map = Session.GetInstance().Map;


            //adapt direction if walls are close
            bool wallRight = this.wallTopRight && this.wallBottomRight;
            bool wallLeft = this.wallTopLeft && this.wallBottomLeft;
            bool wallBottom = this.wallBottomLeft && this.wallBottomRight;
            bool wallTop = this.wallTopLeft && this.wallTopRight;

            Vector2 pos = anubis.CenterPosition();
            Vector2 topLeft = anubis.TopLeftCornerPosition();
            Vector2 topRight = anubis.TopLeftCornerPosition() + new Vector2(anubis.Size().X, 0);
            Vector2 bottomLeft = anubis.TopLeftCornerPosition() + new Vector2(0, anubis.Size().Y);
            Vector2 bottomRight = anubis.TopLeftCornerPosition() + anubis.Size();

            //Debug.WriteLine("positions: " + pos + ", " + topLeft + ", " + topRight + ", " + bottomLeft + ", " + bottomRight);




            //get positions of current tile
            int nodeId = ai.MovementGraph.ToNodeID(pos);
            Vector2 tileLength = Session.GetInstance().Map.TileSize * Session.GetInstance().Map.TileScale;

            //Debug.WriteLine("Positions: " + pos + ", retPos: " + ai.MovementGraph.ToPosition(nodeId));
            //Debug.WriteLine("node id: " + nodeId);



            if (wallTop)
            {
                //Debug.WriteLine("great wall top");
                direction.Y = Math.Max(0, direction.Y);
            }
            else if (wallBottom)
            {
                //Debug.WriteLine("great wall Bottom");
                direction.Y = Math.Min(0, direction.Y);
            }
            else if (wallLeft)
            {
                //Debug.WriteLine("great wall Left");
                direction.X = Math.Max(0, direction.X);
            }
            else if (wallRight)
            {
                //Debug.WriteLine("great wall Right");
                direction.X = Math.Min(0, direction.X);
            }
            else if (this.wallTopLeft)
            {

                //Corner top left of anubis
                //Debug.Write("corner Top Left: ");
                //check in which quadrant the wallSystemPoint lies in the wall tile
                int quadrant = insideTileLocation(this.wallTopLeftPos);

                if (quadrant == 3)
                {
                    //below wall
                    if (direction.X < direction.Y)
                    {
                        //more to the left than top, thus away from the corner
                        direction.Y = Math.Max(0, direction.Y);

                    }
                    else
                    {
                        //direction more to the top than left, thus towards the corner
                        direction = new Vector2(1, 0);
                    }
                    //Debug.WriteLine(" wall above");
                }
                else if (quadrant == 2)
                {
                    //right side of wall
                    //right side of wall
                    if (direction.X < direction.Y)
                    {
                        //more to the left than top, thus towards the corner
                        direction = new Vector2(0, 1);
                    }
                    else
                    {
                        //more to the top than right, thus away from the corner
                        direction.X = Math.Max(0, direction.X);
                    }
                    //Debug.WriteLine(" wall left");
                }
                else
                {
                    Debug.WriteLine("AI: quadrant failed, pls screenshot: topLeft, quad: " + quadrant);
                }
            }
            else if (this.wallTopRight)
            {
                //Debug.Write("corner Top Right: ");

                int quadrant = insideTileLocation(this.wallTopRightPos);

                if (quadrant == 3)
                {
                    //topright point is below the wall
                    if (direction.X > -1 * direction.Y)
                    {
                        //walk more to the right than top, thus away from corner
                        direction.Y = Math.Max(0, direction.Y);

                    }
                    else
                    {
                        //walk more to the top than right, thus towards the corner
                        direction = new Vector2(-1, 0);
                    }
                    //Debug.WriteLine(" Wall above");
                }
                else if (quadrant == 4)
                {
                    //topright position is left side of wall
                    if (direction.X > -1 * direction.Y)
                    {
                        //walk more to the right than top, thus towards the corner
                        direction = new Vector2(0, 1);

                    }
                    else
                    {
                        //walk more to the top than right, thus away from the corner
                        direction.X = Math.Min(0, direction.X);
                    }
                    //Debug.WriteLine(" Wall right");
                } else
                {
                    Debug.WriteLine("AI: quadrant failed, pls screenshot:, topRight quad: " + quadrant);
                }
            }
            else if (this.wallBottomLeft)
            {
                //Debug.Write("corner Bottom Left: ");

                int quadrant = insideTileLocation(this.wallBottomLeftPos);

                if(quadrant == 1)
                {
                    //bottom left position is above wall

                    if (-1 * direction.X > direction.Y)
                    {
                        //walk more to the left than bottom, thus away from corner
                        //just prevent from walking down into wall
                        direction.Y = Math.Min(0, direction.Y);
                    }
                    else
                    {
                        //walk more to the bottom than left, thus into corner
                        direction = new Vector2(1, 0);

                    }
                    //Debug.WriteLine(" Wall below");
                }
                else if (quadrant == 2)
                {
                    //bottom left position is right side of wall

                    if (-1 * direction.X > direction.Y)
                    {
                        //walk more to the left than bottom, thus towards the corner
                        direction = new Vector2(0, -1);
                    }
                    else
                    {
                        //walk more to the bottom than left, thus away from corner
                        direction.X = Math.Max(0, direction.X);
                    }

                    //Debug.WriteLine(" Wall left");
                } else
                {
                    Debug.WriteLine("AI: quadrant failed, pls screenshot: BottomLeft, Quad: " + quadrant);
                }
            }
            else if (this.wallBottomRight)
            {
                //Debug.Write("corner Bottom Right: ");
                int quadrant = insideTileLocation(this.wallBottomRightPos);
                if(quadrant == 1)
                {
                    //bottom right position is above wall
                    if (direction.X > direction.Y)
                    {
                        //walk more towards the right than bottom, thus away from corner
                        direction.Y = Math.Min(0, direction.Y);
                    }
                    else
                    {
                        //walk more towards the bottom than right, thus towards the corner
                        direction = new Vector2(-1, 0);
                    }

                    //Debug.WriteLine(" Wall below");
                } else if (quadrant == 4)
                {
                    //bottom right position is left side of wall
                    if (direction.X > direction.Y)
                    {
                        //walk more towards the right than bottom, thus towards the corner
                        direction = new Vector2(0, -1);
                    }
                    else
                    {
                        //walk more towards the bottom than right, thus away from corner
                        direction.X = Math.Min(0, direction.X);
                    }

                    //Debug.WriteLine(" Wall right");
                }
                else
                {
                    Debug.WriteLine("AI: quadrant failed, pls screenshot: TopRight, quad: " + quadrant);
                }                
            }
            direction.Normalize();
            if (direction != direction_before)
            {
                //Debug.WriteLine("direc augmented (o,n): " + direction_before + ", " + direction);
            }
            return direction;
        }

        Vector2 wallDirectionAugmentation2(AI ai, Vector2 direction)
        {
            direction.Normalize();
            Debug.WriteLine("direction initially: " + direction);
            Entity anubis = ai.Entity;
            Map map = Session.GetInstance().Map;


            //adapt direction if walls are close
            bool wallRight = this.wallTopRight && this.wallBottomRight;
            bool wallLeft = this.wallTopLeft && this.wallBottomLeft;
            bool wallBottom = this.wallBottomLeft && this.wallBottomRight;
            bool wallTop = this.wallTopLeft && this.wallTopRight;

            Vector2 pos = anubis.CenterPosition();
            Vector2 topLeft = anubis.TopLeftCornerPosition();
            Vector2 topRight = anubis.TopLeftCornerPosition() + new Vector2(anubis.Size().X, 0);
            Vector2 bottomLeft = anubis.TopLeftCornerPosition() + new Vector2(0, anubis.Size().Y);
            Vector2 bottomRight = anubis.TopLeftCornerPosition() + anubis.Size();

            //Debug.WriteLine("positions: " + pos + ", " + topLeft + ", " + topRight + ", " + bottomLeft + ", " + bottomRight);




            //get positions of current tile
            int nodeId = ai.MovementGraph.ToNodeID(pos);
            Vector2 tileLength = Session.GetInstance().Map.TileSize * Session.GetInstance().Map.TileScale;

            //Debug.WriteLine("Positions: " + pos + ", retPos: " + ai.MovementGraph.ToPosition(nodeId));
            //Debug.WriteLine("node id: " + nodeId);



            if (wallTop)
            {
                Debug.WriteLine("great wall top");
                direction.Y = Math.Max(0, direction.Y);
            }
            else if (wallBottom)
            {
                Debug.WriteLine("great wall Bottom"); 
                direction.Y = Math.Min(0, direction.Y);
            }
            else if (wallLeft)
            {
                Debug.WriteLine("great wall Left"); 
                direction.X = Math.Max(0, direction.X);
            }
            else if (wallRight)
            {
                Debug.WriteLine("great wall Right"); 
                direction.X = Math.Min(0, direction.X);
            }
            else if (this.wallTopLeft)
            {

                //Corner top left of anubis
                Debug.Write("corner Top Left: ");
                //check if topleft position is below wall (or to the right)
                if (horizontalTileNeigh(ai, topLeft, pos))
                {
                    //top left position is below wall
                    if (direction.X < direction.Y)
                    {
                        //more to the left than top, thus away from the corner
                        direction.Y = Math.Max(0, direction.Y);
                        
                    } else
                    {
                        //direction more to the top than left, thus towards the corner
                        direction = new Vector2(1, 0);
                    }

                    Debug.WriteLine(" wall above");
                }
                else
                {
                    //right side of wall
                    if (direction.X < direction.Y)
                    {
                        //more to the left than top, thus towards the corner
                        direction = new Vector2(0, 1);
                    } else
                    {
                        //more to the top than right, thus away from the corner
                        direction.X = Math.Max(0, direction.X);
                    }
                    Debug.WriteLine(" wall left");
                }
            }
            else if (this.wallTopRight)
            {
                Debug.Write("corner Top Right: ");
                if (horizontalTileNeigh(ai, topRight, pos))
                {
                    //topright point is below the wall
                    if (direction.X > -1 * direction.Y)
                    {
                        //walk more to the right than top, thus away from corner
                        direction.Y = Math.Max(0, direction.Y);

                    } else
                    {
                        //walk more to the top than right, thus towards the corner
                        direction = new Vector2(-1, 0);
                    }
                    Debug.WriteLine(" Wall above");
                }
                else
                {
                    //topright position is left side of wall
                    if (direction.X > -1 * direction.Y)
                    {
                        //walk more to the right than top, thus towards the corner
                        direction = new Vector2(0, 1);

                    } else
                    {
                        //walk more to the top than right, thus away from the corner
                        direction.X = Math.Min(0, direction.X);

                    }
                    Debug.WriteLine(" Wall right");
                }
            }
            else if (this.wallBottomLeft)
            {
                Debug.Write("corner Bottom Left: ");
                if (horizontalTileNeigh(ai, bottomLeft, pos))
                {
                    //bottom left position is above wall

                    if (-1 * direction.X > direction.Y)
                    {
                        //walk more to the left than bottom, thus away from corner
                        //just prevent from walking down into wall
                        direction.Y = Math.Min(0, direction.Y);
                    } else
                    {
                        //walk more to the bottom than left, thus into corner
                        direction = new Vector2(1, 0);

                    }
                    Debug.WriteLine(" Wall below");
                }
                else
                {
                    //bottom left position is right side of wall

                    if (-1 * direction.X > direction.Y)
                    {
                        //walk more to the left than bottom, thus towards the corner
                        direction = new Vector2(0, -1);
                    }
                    else
                    {
                        //walk more to the bottom than left, thus away from corner
                        direction.X = Math.Max(0, direction.X);
                    }

                    Debug.WriteLine(" Wall left");
                }
            }
            else if (this.wallBottomRight)
            {
                Debug.Write("corner Bottom Right: ");
                if (horizontalTileNeigh(ai, bottomRight, pos))
                {
                    //bottom right position is above wall
                    if (direction.X > direction.Y)
                    {
                        //walk more towards the right than bottom, thus away from corner
                        direction.Y = Math.Min(0, direction.Y);
                    } else
                    {
                        //walk more towards the bottom than right, thus towards the corner
                        direction = new Vector2(-1, 0);
                    }

                    Debug.WriteLine(" Wall below");
                }
                else
                {
                    //bottom right position is left side of wall
                    if (direction.X > direction.Y)
                    {
                        //walk more towards the right than bottom, thus towards the corner
                        direction = new Vector2(0, -1);
                    } else
                    {
                        //walk more towards the bottom than right, thus away from corner
                        direction.X = Math.Min(0, direction.X);
                    }

                    Debug.WriteLine(" Wall right");
                }
            }
            direction.Normalize();
            Debug.WriteLine("direction augmented: " + direction);
            return direction;
        }

        Vector2 wallDirectionAugmentation(AI ai, Vector2 direction)
        {
            Entity anubis = ai.Entity;
            Map map = Session.GetInstance().Map;


            //adapt direction if walls are close
            bool wallRight = this.wallTopRight && this.wallBottomRight;
            bool wallLeft = this.wallTopLeft && this.wallBottomLeft;
            bool wallBottom = this.wallBottomLeft && this.wallBottomRight;
            bool wallTop = this.wallTopLeft && this.wallTopRight;

            Vector2 pos = anubis.CenterPosition();
            Vector2 topLeft = anubis.TopLeftCornerPosition();
            Vector2 topRight = anubis.TopLeftCornerPosition() + new Vector2(anubis.Size().X, 0);
            Vector2 bottomLeft = anubis.TopLeftCornerPosition() + new Vector2(0, anubis.Size().Y);
            Vector2 bottomRight = anubis.TopLeftCornerPosition() + anubis.Size();

            //Debug.WriteLine("positions: " + pos + ", " + topLeft + ", " + topRight + ", " + bottomLeft + ", " + bottomRight);




            //get positions of current tile
            int nodeId = ai.MovementGraph.ToNodeID(pos);
            Vector2 tileLength = Session.GetInstance().Map.TileSize * Session.GetInstance().Map.TileScale;

            //Debug.WriteLine("Positions: " + pos + ", retPos: " + ai.MovementGraph.ToPosition(nodeId));
            //Debug.WriteLine("node id: " + nodeId);



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
                    this.randomTileNrSet = false;
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


            direction = wallDirectionAugmentation3(ai, direction) ;

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

        public int GetDistToclosestPlayer(AI ai, Vector2 positionAnubis)
        {
            MovementGraph movementGraph = ai.MovementGraph;
            List<Character> characters = Session.GetInstance().World.GetChildrenOfType<Character>();
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
            List<Character> characters = Session.GetInstance().World.GetChildrenOfType<Character>();

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
            List<Character> characters = Session.GetInstance().World.GetChildrenOfType<Character>();


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

            //Vector2 top = pos + increaser * new Vector2(0, diagVec.Y);
            //Vector2 bottom = pos + new Vector2(0, -1*diagVec.Y);
            //Vector2 left = pos + new Vector2(diagVec.X, 0);
            //Vector2 right = pos + new Vector2(-1*diagVec.X, 0);

            Vector2 topLeft = pos + increaser*diagVec + topIncreaser;
            Vector2 topRight = pos + increaser*new Vector2(-1 * diagVec.X, diagVec.Y) + topIncreaser;
            Vector2 bottomLeft = pos + increaser*new Vector2(diagVec.X, -1 * diagVec.Y);
            Vector2 bottomRight = pos - increaser*diagVec;

            Vector2 top = topLeft + (float)0.5 * (topRight - topLeft);
            Vector2 bottom = bottomLeft + (float)0.3 * (bottomRight - bottomLeft);
            Vector2 left = topLeft + (float)0.5 * (bottomLeft - topLeft);
            Vector2 right = topRight + (float)0.5 * (bottomRight - topRight);



            //update all booleans
            //this.wallTop = !this.validPosition(top);
            //this.wallLeft = !this.validPosition(left); ;
            //this.wallBottom = !this.validPosition(bottom);
            //this.wallRight = !this.validPosition(right);
            this.wallBottomLeft = !this.validPosition(bottomLeft);
            this.wallBottomRight = !this.validPosition(bottomRight);
            this.wallTopLeft = !this.validPosition(topLeft);
            this.wallTopRight = !this.validPosition(topRight);

            this.wallBottomLeftPos = bottomLeft;
            this.wallTopLeftPos = topLeft;
            this.wallTopRightPos = topRight;
            this.wallBottomRightPos = bottomRight;

            if (verbose >= 1)
            {
                if (this.wallTopLeft) { Debug.WriteLine("Wall TopLeft"); };
                if (this.wallTopRight) { Debug.WriteLine("Wall TopRight"); };
                if (this.wallBottomRight) { Debug.WriteLine("Wall BottomRight"); };
                if (this.wallBottomLeft) { Debug.WriteLine("Wall BottomLeft"); };

            }


            //update quadrant

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
                List<Character> characters = Session.GetInstance().World.GetChildrenOfType<Character>();
                List<Artefact> artefacts = Session.GetInstance().World.GetChildrenOfType<Artefact>();

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
                            this.decreaseIncreasgeOfMscuasetailing();
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
                                    this.decreaseIncreasgeOfMscuasetailing();
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
                                //just for saftey:
                                this.decreaseIncreasgeOfMscuasetailing();
                            }

                        }
                    }



                    //after game logic but before direction cal:
                    //check if tailed player is being taled for a longer time:
                    if(this.tailingPlayer && movementGraph.GetDistance(movementGraph.ToNodeID(positionAnubis), movementGraph.ToNodeID(this.tailedPlayer.CenterPosition())) <= this.increaseMsOverTimeDistance) {
                        increaseMsCauseTailing(deltaTime);
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

                        //check if path to that tileNr exists
                        int saftycounter = 0; //should be unneseccary now, but safty first
                        while(!(movementGraph.GetDistance(movementGraph.ToNodeID(positionAnubis), this.randomTileNr) > 0) && saftycounter < 30)
                        {
                            this.randomTileNr = movementGraph.GetRandomTileNr(movementGraph.ToNodeID(positionAnubis));
                            //Debug.WriteLine("adf, after: " + this.randomTileNr + ", dist to it: " + movementGraph.GetDistance(movementGraph.ToNodeID(positionAnubis), this.randomTileNr));
                            saftycounter++;
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

                //set new Anubis Position
                Vector2 shift = entity.TopLeftCornerPosition() - entity.CenterPosition();
                transform.Position = newPosition + shift;

                return;

                
            }
        }
    }
}
