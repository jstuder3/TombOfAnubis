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
        public override void Update(GameTime gameTime)
        {

            // start effects that have not yet started
            foreach (GameplayEffect effect in GetComponents())
            {
                if (!effect.IsStarted())
                {
                    effect.Start(gameTime);
                } 
            }

            List<GameplayEffect> effectsToRemove = new List<GameplayEffect>();

            // check that effects are still active. if not, mark them for deletion
            foreach (GameplayEffect effect in GetComponents()) { 
                if (!effect.IsActive(gameTime))
                {
                    effect.EndGameplayEffect();
                    effectsToRemove.Add(effect);
                }
            }

            // then remove effects that have timed out
            foreach (GameplayEffect effectToRemove in effectsToRemove)
            {
                // delete handles effect disabling ("reverts" effects of applying, if necessary)
                Entity ent = effectToRemove.Entity;
                //effectToRemove.Delete();
                ent.RemoveComponent(effectToRemove);
            }

            // actually apply effect (either "on startup", or continuously, depending on the effect)
            foreach (GameplayEffect effect in GetComponents()) {
                // GameplayEffects now handle their effect-dependent operations on their own
                effect.Update(gameTime);
            }

        }

        

    }
}
