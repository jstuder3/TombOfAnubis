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
            List<GameplayEffect> effectsToRemove = new List<GameplayEffect>();

            // start effects that have not yet started
            foreach (GameplayEffect effect in components)
            {
                if (!effect.IsStarted())
                {
                    effect.Start(gameTime);
                } 
            }

            // check that effects are still active. if not, mark them for deletion
            foreach (GameplayEffect effect in components) { 
                if (!effect.IsActive(gameTime))
                {
                    effectsToRemove.Add(effect);
                }
            }

            // then remove effects that have timed out
            foreach (GameplayEffect effectToRemove in effectsToRemove)
            {
                // delete handles effect disabling ("reverts" effects of applying, if necessary)
                effectToRemove.Delete();
            }

            // actually apply effect (either "on startup", or continuously, depending on the effect)
            foreach (GameplayEffect effect in components) {
                // GameplayEffects now handle their effect-dependent operations on their own
                effect.Update(gameTime);
            }

        }

        

    }
}
