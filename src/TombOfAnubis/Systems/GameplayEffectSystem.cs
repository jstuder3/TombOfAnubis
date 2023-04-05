using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TombOfAnubis
{
    public class GameplayEffectSystem : BaseSystem<GameplayEffect>
    {
        int speedUpModifier = 400;

        public override void Update(GameTime gameTime)
        {
            List<GameplayEffect> effectsToRemove = new List<GameplayEffect>();
            //wait for effect to run out
            foreach (GameplayEffect effect in components)
            {
                //check that effects are still active
                if (!effect.IsActive())
                {
                    switch (effect.type)
                    {
                        case (EffectType.Speedup):
                            //decrease view distance again
                            effect.Entity.GetComponent<Movement>().MaxSpeed -= speedUpModifier;
                            Console.WriteLine("Put MaxSpeed to " + effect.Entity.GetComponent<Movement>().MaxSpeed);
                            break;
                    }
                    effectsToRemove.Add(effect);
                }

            }

            foreach (GameplayEffect effectToRemove in effectsToRemove) { 
                Deregister(effectToRemove);
            }

            //apply effect (either "on startup", or continuously, depending on the effect)
            foreach (GameplayEffect effect in components) {
                if (!effect.HasBeenApplied() || effect.IsAppliedEveryUpdate())
                {
                    switch (effect.type)
                    {
                        case (EffectType.Speedup):
                            //increase movement speed, then mark as applied
                            effect.Entity.GetComponent<Movement>().MaxSpeed += speedUpModifier;
                            effect.Apply();
                            Console.WriteLine("Put MaxSpeed to " + effect.Entity.GetComponent<Movement>().MaxSpeed);
                            break;
                            //case EffectType.Resurrection:
                            //resurrect player
                            //  break;
                    }
                }
            }

        }

        

    }
}
