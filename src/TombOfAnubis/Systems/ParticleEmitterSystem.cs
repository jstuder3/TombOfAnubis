using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis;

namespace TombOfAnubis
{
    public class ParticleEmitterSystem : BaseSystem<ParticleEmitter>
    {
        public override void Update(GameTime gameTime)
        {
        
            List<ParticleEmitter> emittersToRemove = new List<ParticleEmitter>();

            // check that emitters are still active. if not, mark them for deletion
            foreach (ParticleEmitter emitter in components)
            {
                if (!emitter.IsActive())
                {
                    emittersToRemove.Add(emitter);
                }
            }

            // then remove effects that have timed out
            foreach (ParticleEmitter emitterToRemove in emittersToRemove)
            {
                // delete handles effect disabling ("reverts" effects of applying, if necessary)
                emitterToRemove.Delete();
            }

            foreach (ParticleEmitter particleEffect in components)
            {
                particleEffect.Update(gameTime);
            }
        }
    }
}

