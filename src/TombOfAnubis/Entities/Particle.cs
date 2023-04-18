using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public  class Particle : Entity
    {
        public ParticleEmitterConfiguration ParticleConfiguration;

        //public Sprite ParticleSprite;
        //public Transform Transform;
        //public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        //public Vector2 Rotation { get; set; }
        //public Vector2 Scale { get; set; }
        //public Color Tint { get; set; }
        public float SpawnTime { get; set; }
        public float Duration { get; set; }
        public float AliveUntil { get; set; }

        public ParticleEmitter ParentEmitter { get; set; }

        bool HasEnded = false;
        public Vector2 GlobalOffset { get; set; }
        public Vector2 LocalOffset { get; set; }

        static Random random = new Random();


        public Particle(ParticleEmitterConfiguration config, ParticleEmitter emitter)
        {
            ParticleConfiguration = config;
            ParentEmitter = emitter;

            ParticleConfiguration.SpawnDirection.Normalize();

            ResetParticle();
        }

        public void ResetParticle()
        {
            if (ParticleConfiguration.ParticlesMoveWithEntity)
            {
                GlobalOffset = Vector2.Zero;
            }
            else
            {
                GlobalOffset = ParentEmitter.EntityPosition;//Entity.GetComponent<Transform>().Position;
            }
            LocalOffset = ParticleConfiguration.LocalPosition + GenerateUniformlyRandomPointOnDisk();

            Sprite sprite = new Sprite(ParticleConfiguration.Texture, RandomLinearlyInterpolatedTint(), 1);
            AddComponent(sprite);

            Vector2 randomizedScale = ParticleConfiguration.Scale * (Vector2.One + ((float)random.NextDouble() - 0.5f) * ParticleConfiguration.RelativeScaleVariation);

            Transform transform = new Transform(GetGlobalPosition(), randomizedScale);
            AddComponent(transform);

            float SpawnConeRadians = ParticleConfiguration.SpawnConeDegrees / 360f * 2 * MathF.PI;

            float angle = ((float)random.NextDouble() - 0.5f) * SpawnConeRadians;

            float x1 = ParticleConfiguration.SpawnDirection.X;
            float y1 = ParticleConfiguration.SpawnDirection.Y;

            float x2 = MathF.Cos(angle) * x1 - MathF.Sin(angle) * y1;
            float y2 = MathF.Sin(angle) * x1 + MathF.Cos(angle) * y1;

            Velocity = new Vector2(x2, y2) * ParticleConfiguration.InitialSpeed;

            SpawnTime = -1;
            Duration = ParticleConfiguration.ParticleDuration;
            AliveUntil = -1;
            HasEnded = false;

        }

        public Color RandomLinearlyInterpolatedTint()
        {
            float interp = (float)random.NextDouble();
            int red = (int)MathF.Round(ParticleConfiguration.RandomizedTintMin.R * (1 - interp) + ParticleConfiguration.RandomizedTintMax.R * interp);
            int green = (int)MathF.Round(ParticleConfiguration.RandomizedTintMin.G * (1 - interp) + ParticleConfiguration.RandomizedTintMax.G *  interp);
            int blue = (int)MathF.Round(ParticleConfiguration.RandomizedTintMin.B * (1 - interp) + ParticleConfiguration.RandomizedTintMax.B * interp);

            return new Color(red, green, blue);

        }

        public Vector2 GetGlobalPosition()
        {
            return GlobalOffset + LocalOffset;
        }

        public bool IsAlive()
        {
            return !HasEnded;
        }


        public void Update(GameTime gameTime)
        {
            if (HasEnded)
            {
                return;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //on the first update after a reset, set timers
            if (AliveUntil == -1)
            {
                SpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;
                AliveUntil = SpawnTime + Duration;
            }

            if((float)gameTime.TotalGameTime.TotalSeconds > AliveUntil)
            {
                HasEnded = true;
                RemoveComponent(GetComponent<Transform>());
                RemoveComponent(GetComponent<Sprite>());
                return;
            }

            // transform updates
            LocalOffset +=  Velocity * deltaTime;

            Transform transform = GetComponent<Transform>();
            transform.Position = GetGlobalPosition();
            if (ParticleConfiguration.ScalingMode == ScalingMode.LinearDecreaseToZero) {
                if(Duration == 0)
                {
                    throw new Exception("Cannot use ScalingMode.LinearDecreaseToZero with Duration of zero! Please set a duration for the particle if you want to use this scaling mode.");
                }
                transform.Scale = ParticleConfiguration.Scale * ((AliveUntil - (float)gameTime.TotalGameTime.TotalSeconds) / Duration);
                //adjust for non-center origin (because the sprites have origin at the top left, but we want them to "scale down into their center")
                transform.Position += new Vector2(ParticleConfiguration.Texture.Width * ParticleConfiguration.Scale.X / 2f, ParticleConfiguration.Texture.Height* ParticleConfiguration.Scale.Y / 2f) * (1-((AliveUntil - (float)gameTime.TotalGameTime.TotalSeconds) / Duration));
            }

            //physics update
            Velocity += ParticleConfiguration.Gravity * deltaTime;

            if (ParticleConfiguration.PointForceStrength > 0)
            {

                Vector2 directionToPointForce = ParticleConfiguration.LocalPointForcePosition - LocalOffset;
                float distanceToPointForce = directionToPointForce.Length();
                directionToPointForce.Normalize();

                if (ParticleConfiguration.PointForceUsesQuadraticFalloff) //behave more like real-world forces, but is currently buggy
                {
                    Velocity += directionToPointForce * ParticleConfiguration.PointForceStrength * 1f / MathF.Pow(distanceToPointForce, 2f) * deltaTime;
                }
                else //uses no falloff: absolute force is the same everywhere
                {
                    Velocity += directionToPointForce * ParticleConfiguration.PointForceStrength * deltaTime;
                }
            }

            //drag

            Velocity = Velocity * MathF.Pow((1 - ParticleConfiguration.Drag), deltaTime);

            //Console.WriteLine("Position: " + transform.ToWorld().Position + ", Scale: " + transform.ToWorld().Scale);

        }

        public Vector2 GenerateUniformlyRandomPointOnDisk()
        {

            //as we have learned in Computer Graphics, uniformly sampling a disk needs to be done with some transformation magic, otherwise there is more weight towards the center of the disk
            float r = MathF.Sqrt((float)random.NextDouble()) * ParticleConfiguration.RandomizedSpawnPositionRadius;
            float theta = (float)random.NextDouble() * 2 * MathF.PI;

            float x = r * MathF.Cos(theta);
            float y = r * MathF.Sin(theta);

            return new Vector2(x, y);

        }

    }
}
