using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{

    public struct ParticleTextureLibrary
    {
        public static Texture2D BasicParticle;
        public static Texture2D Circle;
        public static Texture2D CircleFilled;
        public static Texture2D CircleFilledWithOutline;
        public static Texture2D SquareFilled;
        public static Texture2D FourCornerStar;
        public static Texture2D FourCornerStarWithOutline;
    }

    //determine how particle sizes should change over their lifespan
    public enum ScalingMode
    {
        Constant,
        LinearDecreaseToZero
    }

    public enum AlphaMode
    {
        Constant,
        LinearDecreaseToZero
    }

    //this takes a LocalPosition, as it can move with the Parent Entity's Position
    public struct ParticleEmitterConfiguration
    {
        public Vector2 LocalPosition { get; set; }
        public float RandomizedSpawnPositionRadius { get; set; }
        //doesn't work yet. set this to false for correct behaviour
        public bool ParticlesMoveWithEntity { get; set; }
        public Texture2D Texture { get; set; }
        public int SpriteLayer { get; set; }
        public float InitialAlpha { get; set; }
        public AlphaMode AlphaMode { get; set; }
        public Color RandomizedTintMin { get; set; }
        public Color RandomizedTintMax { get; set; }
        public Color TintOverLifetimeStart { get; set; }
        public Color TintOverLifetimeEnd { get; set; }
        public Vector2 Scale { get; set; }
        public ScalingMode ScalingMode { get; set; }
        public Vector2 RelativeScaleVariation { get; set; }
        public float EmitterDuration {get; set;}
        public float ParticleDuration { get; set; }
        public float EmissionFrequency { get; set; }
        public float EmissionRate { get; set; }
        public float InitialSpeed { get; set; }
        public Vector2 SpawnDirection { get; set; }
        public float SpawnConeDegrees { get; set; }

        public Vector2 Gravity { get; set; }
        public Vector2 LocalPointForcePosition { get; set; }
        public float PointForceStrength { get; set; }
        public bool PointForceUsesQuadraticFalloff { get; set; }
        public float Drag { get; set; }

    }


    //ParticleEffects should be attached either to the scene or to an entity
    public class ParticleEmitter : Component
    {
        //public static Texture2D FourCornerStarWithOutline;

        public ParticleEmitterConfiguration EmitterConfiguration;

        public float StartTime = 0;
        public float AliveUntil = -1;
        public bool HasEnded = false;

        public List<Particle> ParticleList;
        public int FirstDeadParticleIndex;

        public float LastBurstTime;

        public Vector2 EntityPosition { get; set; }

        public ParticleEmitter(ParticleEmitterConfiguration config)
        {
            EmitterConfiguration = config;
            ParticleList = new List<Particle>();
            ParticleEmitterSystem.Register(this);
        }

        public static void LoadContent(GameScreenManager gameScreenManager)
        {
            ContentManager content = gameScreenManager.Game.Content;

            // load textures into static variables of ParticleTextureLibrary, so they can be used as a texture library
            string particles_base_path = "Textures/Objects/Particles/";
            ParticleTextureLibrary.BasicParticle = content.Load<Texture2D>(particles_base_path+"basic_particle");
            ParticleTextureLibrary.Circle = content.Load<Texture2D>(particles_base_path + "circle");
            ParticleTextureLibrary.CircleFilled = content.Load<Texture2D>(particles_base_path + "circle_filled");
            ParticleTextureLibrary.CircleFilledWithOutline = content.Load<Texture2D>(particles_base_path + "circle_filled_with_outline");
            ParticleTextureLibrary.SquareFilled = content.Load<Texture2D>(particles_base_path + "square_filled");
            ParticleTextureLibrary.FourCornerStar = content.Load<Texture2D>(particles_base_path + "four_corner_star");
            ParticleTextureLibrary.FourCornerStarWithOutline = content.Load<Texture2D>(particles_base_path + "four_corner_star_with_outline");
        }

        public override void Delete()
        {
            base.Delete();

            //finally delete all particles and deregister emitter
            foreach (Particle particle in ParticleList)
            {
                particle.Delete();
            }

            ParticleList.Clear();
            ParticleList = null;
            
            ParticleEmitterSystem.Deregister(this);

        }

        public void EndEmitter()
        {
            //the particles that remain shouldn't just be destroyed. We keep updating until they have all ran out, and only then the ParticleEmitter and all the particles get destroyed
            AliveUntil = StartTime;
            HasEnded = true;
        }

        public bool IsActive()
        {
            return !(HasEnded && FirstDeadParticleIndex == 0);
        }

        public void Update(GameTime gameTime)
        {
            //Console.WriteLine("Total number of particles: " + ParticleList.Count);
            //Console.WriteLine("Currently alive particles: " + FirstDeadParticleIndex);

            EntityPosition = Entity.TopLeftCornerPosition();

            //check that the overall particle effect hasn't timed out. if it has, deregister, but leave the particles that exist alive until they all run out as to not make them all disappear all at once in an ugly fashion
            if (AliveUntil == -1)
            {
                StartTime = (float)gameTime.TotalGameTime.TotalSeconds;
                if (EmitterConfiguration.EmitterDuration > 0)
                {
                    AliveUntil = StartTime + EmitterConfiguration.EmitterDuration;
                }
                else
                {
                    AliveUntil = float.MaxValue;
                }
                LastBurstTime = StartTime;

            }

            if((float)gameTime.TotalGameTime.TotalSeconds > AliveUntil)
            {
                HasEnded = true;
            }

            //update all alive particles
            //note: update happens first, so we don't overspawn new particles in case some of them die during the current update
            for(int i = 0; i < FirstDeadParticleIndex; i++)
            {
                Particle currentParticle = ParticleList[i];

                //check if the particle is still alive. if not, swap positions with the last alive particle, then reduce FirstDeadParticleIndex by one
                if(!currentParticle.IsAlive())
                {
                    Particle lastAliveParticle = ParticleList[FirstDeadParticleIndex - 1];
                    ParticleList[FirstDeadParticleIndex - 1] = currentParticle;
                    ParticleList[i] = lastAliveParticle;
                    currentParticle = lastAliveParticle;
                    FirstDeadParticleIndex--;
                }
                //then update (note: not yet implemented, and to-be-moved here since entities shouldn't update themselves)
                currentParticle.Update(gameTime);
            }

            if (!HasEnded)
            {
                // spawn as many particles as the spawn frequency requires
                float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds - LastBurstTime;
                float burstTime = 1f / EmitterConfiguration.EmissionFrequency;

                int numBurstsToExecute = (int)MathF.Floor(deltaTime / burstTime);

                if (numBurstsToExecute > 0)
                {
                    // check if there are enough dead particles to recycle. if not, spawn some new ones
                    int numParticlesToSpawn = (int)MathF.Round(numBurstsToExecute * EmitterConfiguration.EmissionRate);
                    int numDeadParticles = ParticleList.Count - FirstDeadParticleIndex;

                    while (numParticlesToSpawn > 0)
                    {
                        if (numDeadParticles > 0) //recycle
                        {
                            //reset particle
                            ParticleList[FirstDeadParticleIndex].ResetParticle();
                            FirstDeadParticleIndex++;
                            numDeadParticles--;
                            numParticlesToSpawn--;
                        }
                        else //spawn new particles and append to list
                        {
                            Particle newParticle = new Particle(EmitterConfiguration, this);
                            ParticleList.Insert(FirstDeadParticleIndex, newParticle);
                            if (EmitterConfiguration.ParticlesMoveWithEntity)
                            {
                                Entity.AddChild(newParticle);
                            }
                            else
                            {
                                Session.GetInstance().World.AddChild(newParticle);
                            }
                            numParticlesToSpawn--;
                            FirstDeadParticleIndex++;
                            
                        }
                        /*Session.GetInstance().Scene.AddChild(new Fist(Entity.GetComponent<Transform>().Position, new Vector2(1f, 0f)));
                        numParticlesToSpawn--;
                        */
                    }

                    // update last burst time to when the most recent burst would have happened
                    LastBurstTime += burstTime * numBurstsToExecute; //(float)gameTime.TotalGameTime.TotalSeconds;

                }
            }
        }
    }
}
