﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System; using System.Diagnostics;
using System.ComponentModel.Design.Serialization;
using Microsoft.Xna.Framework.Input;

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
        DelayedFollow,
        TeleportPreview
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
     * TeleportPreview : float teleportdistance (forward vector is directly taken from Entity)
     **/

    public class GameplayEffect : Component
    {
        private bool started;
        public float TimeAlive;
        public float Duration = -1;

        private bool applied;

        //effect-dependent additional variables
        private List<float> effectFloatParameters;
        private List<Vector2> effectVectorParameters;

        private ParticleEmitter stunnedEmitter;
        private Vector2 previousForwardVector;
        private ParticleEmitter teleportPreviewEmitter;
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
            this.Duration = duration;
            started = false;

            this.effectFloatParameters = effectFloatParameters;
            this.effectVectorParameters = effectVectorParameters;
            visibility = Visibility;
            GameplayEffectSystem.Register(this);
        }
        public void Start(GameTime gameTime)
        {
            if (Duration > 0)
            {
                //do nothing
            }
            else
            {
                Duration = float.MaxValue;
            }
            started = true;
        }
        public bool IsStarted()
        {
            return started;
        }
        public bool IsActive(GameTime gameTime)
        {
            return TimeAlive < Duration;
        }

        public bool HasEnded()
        {
            return TimeAlive >= Duration;
        }

        public bool HasBeenApplied()
        {
            return applied;
        }

        public void Update(GameTime gameTime)
        {
            //update local time
            TimeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                        Debug.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
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
                        Debug.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                    }
                    break;
                case EffectType.LinearAutoMove: // applied on every update, so we don't check for applied
                    // move entity in the given direction every update (note that direction is normalized)
                    CheckHasFloatParameters(1);
                    CheckHasVectorParameters(1);
                    if (effectVectorParameters[0] != Vector2.Zero)
                    {
                        effectVectorParameters[0].Normalize();
                        Entity.GetComponent<Transform>().Position += effectVectorParameters[0] * effectFloatParameters[0] * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        Debug.WriteLine("You fed a zero vector to LinearAutoMove. Feeding a zero vector to LinearAutoMove is completely useless and should be avoided!");
                    }
                    //Debug.WriteLine("AutoMoved by " + effectVectorParameters[0] * effectFloatParameters[0] * (float)gameTime.ElapsedGameTime.TotalSeconds + " units. New position: " + Entity.GetComponent<Transform>().Position);
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
                        pec.Visibility = Visibility.Game;
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

                        //make the player slightly transparent
                        Entity.GetComponent<Sprite>().Alpha = 0.4f;

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
                case EffectType.TeleportPreview:
                    if (!applied)
                    {
                        CheckHasFloatParameters(1);
                    }
                    InventorySlot inventorySlot = Entity.GetComponent<Inventory>().GetFullItemSlot();
                    if (IsActive(gameTime) && inventorySlot == null || inventorySlot.Item.ItemType != ItemType.Teleport) { EndGameplayEffect(); break; }

                    movement = Entity.GetComponent<Movement>();
                    if (previousForwardVector != movement.GetForwardVector())
                    {
                        if(teleportPreviewEmitter != null)
                        {
                            previousForwardVector = movement.GetForwardVector();
                            teleportPreviewEmitter.EndEmitter();
                            ParticleEmitterConfiguration prev_pec = teleportPreviewEmitter.EmitterConfiguration;
                            prev_pec.LocalPosition = effectFloatParameters[0] * movement.GetForwardVector();
                            teleportPreviewEmitter = new ParticleEmitter(prev_pec);
                            Entity.AddComponent(teleportPreviewEmitter);
                        }
                        else
                        {
                            previousForwardVector = movement.GetForwardVector();

                            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
                            pec.LocalPosition = effectFloatParameters[0] * movement.GetForwardVector();
                            pec.RandomizedSpawnPositionRadius = 10f;
                            pec.Texture = ParticleTextureLibrary.BasicParticle;
                            pec.SpriteLayer = 1;
                            pec.RandomizedTintMin = Color.Purple;
                            pec.RandomizedTintMax = Color.Blue;
                            pec.Scale = Vector2.One * 0.4f;
                            pec.ScalingMode = ScalingMode.Constant;
                            pec.InitialAlpha = 1f;
                            pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
                            pec.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
                            pec.EmitterDuration = 0f;
                            pec.ParticleDuration = 1f;
                            pec.EmissionFrequency = 20f;
                            pec.EmissionRate = 1f;
                            pec.InitialSpeed = 50f;
                            pec.SpawnDirection = new Vector2(0f, -1f);
                            pec.SpawnConeDegrees = 360f;
                            pec.Drag = 0.5f;

                            teleportPreviewEmitter = new ParticleEmitter(pec);
                            Entity.AddComponent(teleportPreviewEmitter);
                        }
                    }

                    break;
                    
            }

            applied = true;
        }

        public void Stop()
        {
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
                        Debug.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                        break;
                    case EffectType.MultiplicativeSpeedModification:
                        // decrease movement speed again
                        CheckHasFloatParameters(1);
                        movement = Entity.GetComponent<Movement>();
                        movement.MultiplicativeSpeedModifier /= effectFloatParameters[0]; //first float parameter contains speedup
                        movement.UpdateMovementSpeed();
                        Debug.WriteLine("Put MaxSpeed to " + Entity.GetComponent<Movement>().MaxSpeed);
                        break;
                    case EffectType.LinearAutoMove:
                        // stop auto move, so no effect
                        break;
                    case EffectType.Lifetime:
                        //Entity.RemoveComponentWithoutDeleting(this);
                        Entity.Delete();
                        break;
                    case EffectType.Stunned:
                        Entity.GetComponent<Movement>().State = MovementState.Idle;
                        stunnedEmitter.EndEmitter();
                        break;
                    case EffectType.Hidden:
                        movement = Entity.GetComponent<Movement>();
                        movement.State = MovementState.Idle;
                        movement.HiddenFromAnubis = false;
                        Entity.GetComponent<Sprite>().Alpha = 1f;
                        break;
                    case EffectType.OnCooldown:
                         ((ICooldown)Entity).EndCooldown();                      
                        break;
                    case EffectType.TeleportPreview:
                        if (teleportPreviewEmitter != null)
                        {
                            teleportPreviewEmitter.EndEmitter();
                        }
                        break;
                }

            }
        }

        public override void Delete()
        {
            GameplayEffectSystem.Deregister(this);
            base.Delete();
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
                //Debug.WriteLine();
                throw new System.ArgumentOutOfRangeException("effectFloatParameters", "GameplayEffect doesn't have enough Vector parameters! Please make sure you passed all parameters as defined in GameplayEffects.cs when creating the GameplayEffect!");
            }
        }

        public void EndGameplayEffect()
        {
            //"mark" GameplayEffect for deletion on the next update
            Duration = 0;
        }


    }
}
