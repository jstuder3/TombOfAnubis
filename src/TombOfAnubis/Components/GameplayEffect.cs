using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.ComponentModel.Design.Serialization;

namespace TombOfAnubis {
    public enum EffectType
    {
        AdditiveSpeedup,
        MultiplicativeSpeedup,
        IncreaseViewDistance,
        Resurrection,
        TemporarilyStopped,
        AutoMove,
        Lifetime,
        Stunned
    }

    /**
     * EffectType parameters: (additional parameters that must be passed when creating the effect)
     * 
     * Speedup: float speedup
     * IncreaseViewDistance: float viewDistanceIncrease
     * Resurrection: no parameters
     * TemporarilyStopped: no parameters
     * AutoMove: float speed, Vector2 direction
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

        public EffectType Type { get; set; }

        //Different constructors for varying types of parameters. Check the comment above to find out which parameters are necessary for the respective EffectType
        public GameplayEffect(EffectType type, float duration) : this(type, duration, new List<float>(), new List<Vector2>()) { }

        public GameplayEffect(EffectType type, float duration, float effectFloatParameter_01) : this(type, duration, new List<float> { effectFloatParameter_01 }, new List<Vector2>()) { }

        public GameplayEffect(EffectType type, float duration, float effectFloatParameter_01, Vector2 effectVectorParameter_01) : this(type, duration, new List<float> { effectFloatParameter_01 }, new List<Vector2> { effectVectorParameter_01 }) { }

        // main constructor that is called by all the other constructors
        public GameplayEffect(EffectType type, float duration, List<float> effectFloatParameters, List<Vector2> effectVectorParameters)
        {
            Type = type;
            this.duration = duration;
            started = false;

            this.effectFloatParameters = effectFloatParameters;
            this.effectVectorParameters = effectVectorParameters;

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

        public bool HasBeenApplied()
        {
            return applied;
        }

        public void Update(GameTime gameTime)
        {
            // apply effect-dependent effect
            switch (Type)
            {
                case EffectType.AdditiveSpeedup:
                    // increase movement speed once
                    if (!applied) // applied only once, so we check whether it has already been applied before
                    {
                        CheckHasFloatParameters(1);
                        Entity.GetComponent<Movement>().MaxSpeed += (int)effectFloatParameters[0]; //first float parameter contains speedup
                        Console.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    }
                    break;
                case EffectType.MultiplicativeSpeedup:
                    // increase movement speed once
                    if (!applied) // applied only once, so we check whether it has already been applied before
                    {
                        CheckHasFloatParameters(1);
                        Entity.GetComponent<Movement>().MaxSpeed = (int)MathF.Round(Entity.GetComponent<Movement>().MaxSpeed * effectFloatParameters[0]); //first float parameter contains speedup multiplier
                        Console.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    }
                    break;
                case EffectType.AutoMove: // applied on every update, so we don't check for applied
                    // move entity in the given direction every update (note that direction is normalized)
                    CheckHasFloatParameters(1);
                    CheckHasVectorParameters(1);
                    effectVectorParameters[0].Normalize();
                    Entity.GetComponent<Transform>().Position += effectVectorParameters[0] * effectFloatParameters[0] * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Console.WriteLine("AutoMoved by " + effectVectorParameters[0] * effectFloatParameters[0] * (float)gameTime.ElapsedGameTime.TotalSeconds + " units. New position: " + Entity.GetComponent<Transform>().Position);
                    break;
                case EffectType.Lifetime:
                    //destroy the entity once this effect runs out, so this has no effect during Update()
                    break;
                case EffectType.Stunned:
                    if (!applied)
                    {
                        //set the "stunned" flag of the entity
                        Entity.GetComponent<Movement>().State = MovementState.Stunned;
                    }
                    break;
            }

            applied = true;
        }

        public override void Delete()
        {
            // remove effect-dependent effect, then deregister
            switch (Type)
            {
                case EffectType.AdditiveSpeedup:
                    // decrease movement speed again
                    CheckHasFloatParameters(1);
                    Entity.GetComponent<Movement>().MaxSpeed -= (int)effectFloatParameters[0]; //first float parameter contains speedup
                    Console.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    break;
                case EffectType.MultiplicativeSpeedup:
                    // decrease movement speed again
                    CheckHasFloatParameters(1);
                    Entity.GetComponent<Movement>().MaxSpeed = (int)MathF.Round(Entity.GetComponent<Movement>().MaxSpeed / effectFloatParameters[0]); //first float parameter contains speedup
                    Console.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    break;
                case EffectType.AutoMove:
                    // stop auto move, so no effect
                    break;
                case EffectType.Lifetime:
                    // destroy the parent entity once this effect runs out. Notably, we have to remote the gameplayeffect manually first because otherwise Delete() is infinitely recursed
                    Entity.RemoveComponentWithoutDeleting(this);
                    Entity.Delete();
                    break;
                case EffectType.Stunned:
                    Entity.GetComponent<Movement>().State = MovementState.Idle;
                    break;
            }

            GameplayEffectSystem.Deregister(this);
        }

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
            endTime = startTime - 1f;
        }



    }
}
