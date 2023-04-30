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
        public static Texture2D Plus;
        public static Texture2D PlusFilled;
        public static Texture2D PlusFilledWithOutline;

        public static void LoadContent(GameScreenManager gameScreenManager)
        {
            ContentManager content = gameScreenManager.Game.Content;

            // load textures into static variables of ParticleTextureLibrary, so they can be used as a texture library
            string particles_base_path = "Textures/Objects/Particles/";
            ParticleTextureLibrary.BasicParticle = content.Load<Texture2D>(particles_base_path + "basic_particle");
            ParticleTextureLibrary.Circle = content.Load<Texture2D>(particles_base_path + "circle");
            ParticleTextureLibrary.CircleFilled = content.Load<Texture2D>(particles_base_path + "circle_filled");
            ParticleTextureLibrary.CircleFilledWithOutline = content.Load<Texture2D>(particles_base_path + "circle_filled_with_outline");
            ParticleTextureLibrary.SquareFilled = content.Load<Texture2D>(particles_base_path + "square_filled");
            ParticleTextureLibrary.FourCornerStar = content.Load<Texture2D>(particles_base_path + "four_corner_star");
            ParticleTextureLibrary.FourCornerStarWithOutline = content.Load<Texture2D>(particles_base_path + "four_corner_star_with_outline");
            ParticleTextureLibrary.Plus = content.Load<Texture2D>(particles_base_path + "plus"); ;
            ParticleTextureLibrary.PlusFilled = content.Load<Texture2D>(particles_base_path + "plus_filled"); ;
            ParticleTextureLibrary.PlusFilledWithOutline = content.Load<Texture2D>(particles_base_path + "plus_filled_with_outline"); ;
        }

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
        //local offset w.r.t. the parent entity
        public Vector2 LocalPosition { get; set; }
        //in how large of a radius around the position of the center should particles be spawned
        public float RandomizedSpawnPositionRadius { get; set; }
        //whether particles should move when the parent entity moves, rather than be attached to the world. doesn't work yet
        public bool ParticlesMoveWithEntity { get; set; }
        // give the particle a texture that will be used for the sprites
        public Texture2D Texture { get; set; }
        // the layer that the particle sprites will be drawn on
        public int SpriteLayer { get; set; }
        //how transparent the particle should be at the start of its life. 0 means fully transparent, 1 means opaque
        public float InitialAlpha { get; set; }
        //the alpha can either be fixed, or reduce to zero over the lifespan of the particle
        public AlphaMode AlphaMode { get; set; }
        //define a range of colors the particles can spawn with. If you only want one color, set both to the same value
        public Color RandomizedTintMin { get; set; }
        public Color RandomizedTintMax { get; set; }
        //scale of the particle
        public Vector2 Scale { get; set; }
        //the scale can either be fixed, or reduce to zero over the lifespan of the particle
        public ScalingMode ScalingMode { get; set; }
        //by how much the scale can differ from the base scale, relatively. 0 means no variation, 1 means the scale can be 0-2x the base scale. anything outside 0-1 is undefined
        public Vector2 RelativeScaleVariation { get; set; }
        //how long the emitter should be active for. 0 means infinite
        public float EmitterDuration {get; set;}
        //how long each particle should exist for
        public float ParticleDuration { get; set; }
        //how often the emitter should emit particles per second. recommended range: 0.1-60
        public float EmissionFrequency { get; set; }
        //how many particles should be emitted per burst. recommended range: 1-10
        public float EmissionRate { get; set; }
        //how fast the particles should move when they spawn. 0 means no initial movement
        public float InitialSpeed { get; set; }
        //direction in which the particles should be shot in.
        public Vector2 SpawnDirection { get; set; }
        //how much the actual spawn direction is allowed to differ from the provided spawn direction. 0 means they all spawn in the same direction, 360 means they spawn in all directions
        public float SpawnConeDegrees { get; set; }
        //acceleration into a specific direction
        public Vector2 Gravity { get; set; }
        //define point force that can attract/push away particles. currently, this just stays in place for every particle. later, this should move with the attached entity (e.g. so particles are attracted by the player)
        public Vector2 LocalPointForcePosition { get; set; }
        //positive means attracting, negative means repelling
        public float PointForceStrength { get; set; }
        //whether the force should be constant regardless of distance, or whether it should behave move like real gravity, where the force reduces quadratically with distance. currently doesn't really behave controllably, so just leave off.
        public bool PointForceUsesQuadraticFalloff { get; set; }
        //by how much the particles should lose velocity per second. 0 means they don't lose velocity, 0.5 means they lose half their velocity per second, 1 means they lose all of their velocity in one second
        public float Drag { get; set; }
        public Visibility Visibility { get; set; }

    }


    //ParticleEffects should be attached either to the scene or to an entity
    public class ParticleEmitter : Component
    {
        //public static Texture2D FourCornerStarWithOutline;

        public ParticleEmitterConfiguration EmitterConfiguration;

        public float AliveUntil = -1;
        public bool HasEnded = false;

        public List<Particle> ParticleList;
        public int FirstDeadParticleIndex;

        public float LastBurstTime;
        public float TimeAlive = 0;

        public Vector2 EntityPosition { get; set; }

        public ParticleEmitter(ParticleEmitterConfiguration config)
        {
            EmitterConfiguration = config;
            ParticleList = new List<Particle>();
            ParticleEmitterSystem.Register(this);
        }

        public override void Delete()
        {
            base.Delete();

            //finally delete all particles and deregister emitter
            if (ParticleList != null)
            {
                foreach (Particle particle in ParticleList)
                {
                    particle.Delete();
                }
                ParticleList.Clear();
                ParticleList = null;
            }

            ParticleEmitterSystem.Deregister(this);

        }

        public void EndEmitter()
        {
            //the particles that remain shouldn't just be destroyed. We keep updating until they have all ran out, and only then the ParticleEmitter and all the particles get destroyed
            AliveUntil = 0;
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
                if (EmitterConfiguration.EmitterDuration > 0)
                {
                    AliveUntil = EmitterConfiguration.EmitterDuration;
                }
                else
                {
                    AliveUntil = float.MaxValue;
                }
                TimeAlive = 0;
                LastBurstTime = 0;

            }

            //update local time
            TimeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(TimeAlive > AliveUntil)
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
                //then update (note: entities shouldn't update themselves, so the logic should be moved here later)
                currentParticle.Update(gameTime);
            }


            if (!HasEnded)
            {
                // spawn as many particles as the spawn frequency requires
                float deltaTime = TimeAlive - LastBurstTime;
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
