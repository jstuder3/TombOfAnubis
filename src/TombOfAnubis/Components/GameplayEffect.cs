using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.ComponentModel.Design.Serialization;

namespace TombOfAnubis {
    public enum EffectType
    {
        AdditiveSpeedModification,
        MultiplicativeSpeedModification,
        IncreaseViewDistance,
        LinearAutoMove,
        Lifetime,
        Stunned,
        Hidden,
        OnCooldown,
        DelayedFollow
    }

    /**
     * EffectType parameters: (additional parameters that must be passed when creating the effect, besides duration)
     * 
     * AdditiveSpeedModification: float absoluteSpeedModification
     * MultiplicativeSpeedModification: flolat multiplicativeSpeedModification
     * IncreaseViewDistance: float viewDistanceIncrease
     * Resurrection: no parameters
     * AutoMove: float speed, Vector2 direction
     * Stunned: no parameters
     * Hidden: no parameters
     * DelayedFollow: float delayAmount (0-1, where 0 means no delay, 1 means no movement), Entity otherEntity (entity that should be followed)
     **/

    public class GameplayEffect : Component
    {
        private bool started;
        private readonly float duration;
        private float startTime;
        private float endTime;

        private bool applied;

        //effect-dependent additional variables
        private List<float> effectFloatParameters;
        private List<Vector2> effectVectorParameters;

        private ParticleEmitter stunnedEmitter;

        public EffectType Type { get; set; }
        public Entity OtherEntity { get; set; }

        //Different constructors for varying types of parameters. Check the comment above to find out which parameters are necessary for the respective EffectType
        public GameplayEffect(EffectType type, float duration, Visibility visibility) : this(type, duration, new List<float>(), new List<Vector2>(), visibility) { }

        public GameplayEffect(EffectType type, float duration, float effectFloatParameter_01, Visibility visibility) : this(type, duration, new List<float> { effectFloatParameter_01 }, new List<Vector2>(), visibility) { }

        public GameplayEffect(EffectType type, float duration, float effectFloatParameter_01, Vector2 effectVectorParameter_01, Visibility visibility) : this(type, duration, new List<float> { effectFloatParameter_01 }, new List<Vector2> { effectVectorParameter_01 }, visibility) { }

        public GameplayEffect(EffectType type, float duration, float effectFloatParameter_01, Entity otherEntity, Visibility visibility) : this(type, duration, new List<float> { effectFloatParameter_01 }, new List<Vector2>(), visibility) 
        {
            OtherEntity = otherEntity;
        }

        // main constructor that is called by all the other constructors
        public GameplayEffect(EffectType type, float duration, List<float> effectFloatParameters, List<Vector2> effectVectorParameters, Visibility visibility)
        {
            Type = type;
            this.duration = duration;
            started = false;

            this.effectFloatParameters = effectFloatParameters;
            this.effectVectorParameters = effectVectorParameters;
            visibility = Visibility;
            GameplayEffectSystem.Register(this);
        }
        public void Start(GameTime gameTime)
        {
            startTime = (float)gameTime.TotalGameTime.TotalSeconds;
            endTime = startTime + duration;
            started = true;
        }
        public bool IsStarted()
        {
            return started;
        }
        public bool IsActive(GameTime gameTime)
        {
            return (float)gameTime.TotalGameTime.TotalSeconds < endTime;
        }

        public bool HasEnded()
        {
            return startTime == endTime;
        }

        public bool HasBeenApplied()
        {
            return applied;
        }

        public void Update(GameTime gameTime)
        {
            // apply effect-dependent effect
            Movement movement;
            switch (Type)
            {
                case EffectType.AdditiveSpeedModification:
                    // increase movement speed once
                    if (!applied) // applied only once, so we check whether it has already been applied before
                    {
                        CheckHasFloatParameters(1);
                        movement = Entity.GetComponent<Movement>();
                        movement.AdditiveSpeedModifier += effectFloatParameters[0]; //first float parameter contains speedup value
                        movement.UpdateMovementSpeed();
                        Console.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    }
                    break;
                case EffectType.MultiplicativeSpeedModification:
                    // increase movement speed once
                    if (!applied) // applied only once, so we check whether it has already been applied before
                    {
                        CheckHasFloatParameters(1);
                        movement = Entity.GetComponent<Movement>();
                        movement.MultiplicativeSpeedModifier *= effectFloatParameters[0]; //first float parameter contains speedup multiplier
                        movement.UpdateMovementSpeed();
                        Console.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    }
                    break;
                case EffectType.LinearAutoMove: // applied on every update, so we don't check for applied
                    // move entity in the given direction every update (note that direction is normalized)
                    CheckHasFloatParameters(1);
                    CheckHasVectorParameters(1);
                    effectVectorParameters[0].Normalize();
                    Entity.GetComponent<Transform>().Position += effectVectorParameters[0] * effectFloatParameters[0] * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //Console.WriteLine("AutoMoved by " + effectVectorParameters[0] * effectFloatParameters[0] * (float)gameTime.ElapsedGameTime.TotalSeconds + " units. New position: " + Entity.GetComponent<Transform>().Position);
                    break;
                case EffectType.Lifetime:
                    //destroy the entity once this effect runs out, so this has no effect during Update()
                    break;
                case EffectType.Stunned:
                    if (!applied)
                    {
                        //set the "stunned" flag of the entity
                        Entity.GetComponent<Movement>().State = MovementState.Stunned;
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
                        pec.EmitterDuration = 0f;
                        pec.ParticleDuration = 1f;
                        pec.EmissionFrequency = 30f;
                        pec.EmissionRate = 1f;
                        pec.InitialSpeed = 10f;
                        pec.SpawnDirection = new Vector2(0f, -1f);
                        pec.SpawnConeDegrees = 360f;
                        pec.Drag = 0.5f;
                        stunnedEmitter = new ParticleEmitter(pec);
                        Entity.AddComponent(stunnedEmitter);
                    }
                    break;
                case EffectType.Hidden:
                    if (!applied)
                    {
                        //set the "stunned" flag of the entity
                        movement = Entity.GetComponent<Movement>();
                        movement.State = MovementState.Hiding;
                        movement.HiddenFromAnubis = true;
                    }
                    break;
                case EffectType.OnCooldown:
                    if (!applied)
                    {
                        ((ICooldown)Entity).PutOnCooldown();
                    }
                    break;
                case EffectType.DelayedFollow:
                    if (!applied)
                    {
                        CheckHasFloatParameters(1);
                    }
                    Transform entityTransform = Entity.GetComponent<Transform>();
                    Transform otherEntityTransform = OtherEntity.GetComponent<Transform>();

                    Vector2 direction = otherEntityTransform.Position - entityTransform.Position;
                    entityTransform.Position += direction * (1 - effectFloatParameters[0]) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
            }

            applied = true;
        }

        public override void Delete()
        {
            // remove effect-dependent effect, then deregister
            Movement movement;
            switch (Type)
            {
                case EffectType.AdditiveSpeedModification:
                    // decrease movement speed again
                    CheckHasFloatParameters(1);
                    movement = Entity.GetComponent<Movement>();
                    movement.AdditiveSpeedModifier -= effectFloatParameters[0]; //first float parameter contains speedup
                    movement.UpdateMovementSpeed();
                    Console.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    break;
                case EffectType.MultiplicativeSpeedModification:
                    // decrease movement speed again
                    CheckHasFloatParameters(1);
                    movement = Entity.GetComponent<Movement>();
                    movement.MultiplicativeSpeedModifier /= effectFloatParameters[0]; //first float parameter contains speedup
                    movement.UpdateMovementSpeed();
                    Console.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    break;
                case EffectType.LinearAutoMove:
                    // stop auto move, so no effect
                    break;
                case EffectType.Lifetime:
                    // destroy the parent entity once this effect runs out. Notably, we have to remote the gameplayeffect manually first because otherwise Delete() is infinitely recursed
                    if (startTime < endTime) { //this means that the effect was terminated prematurely, in which case we don't forcefully delete this because we are iterating over the very list this effect would manipulate 
                        //do nothing
                    }
                    else
                    {
                        Entity.RemoveComponentWithoutDeleting(this);
                        Entity.Delete();
                    }
                    break;
                case EffectType.Stunned:
                    Entity.GetComponent<Movement>().State = MovementState.Idle;
                    stunnedEmitter.EndEmitter();
                    break;
                case EffectType.Hidden:
                    movement = Entity.GetComponent<Movement>();
                    movement.State = MovementState.Idle;
                    movement.HiddenFromAnubis = false;
                    break;
                case EffectType.OnCooldown:
                     ((ICooldown)Entity).EndCooldown();
                    break;
            }

            GameplayEffectSystem.Deregister(this);
        }

        /*public void DeleteWithoutRevertingEffect()
        {
            GameplayEffectSystem.Deregister(this);
        }*/

        private bool CheckHasFloatParameters(int numRequiredParameters)
        {
            if (effectFloatParameters.Count >= numRequiredParameters) return true;
            else 
            {    
                throw new System.ArgumentOutOfRangeException("effectFloatParameters", "GameplayEffect doesn't have enough float parameters! Please make sure you passed all parameters as defined in GameplayEffects.cs when creating the GameplayEffect!");
            }
        }

        private bool CheckHasVectorParameters(int numRequiredParameters)
        {
            if (effectVectorParameters.Count >= numRequiredParameters) return true;
            else
            {
                Console.WriteLine();
                throw new System.ArgumentOutOfRangeException("effectFloatParameters", "GameplayEffect doesn't have enough Vector parameters! Please make sure you passed all parameters as defined in GameplayEffects.cs when creating the GameplayEffect!");
            }
        }

        public void EndGameplayEffect()
        {
            //"mark" GameplayEffect for deletion on the next update
            endTime = startTime;
        }


    }
}
