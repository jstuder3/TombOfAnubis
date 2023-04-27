using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class MovementSystem : BaseSystem<Movement>
    {
        public override void Update(GameTime deltaTime)
        {
            foreach (Movement movement in GetComponents())
            {
                Entity entity = movement.Entity;
                Animation animation = entity.GetComponent<Animation>();
                if (animation != null && entity.GetType() != typeof(Ghost))
                {
                    if(movement.State == MovementState.Walking)
                    {
                        animation.Resume();
                        switch (movement.Orientation) 
                        {
                            case Orientation.Up:
                                animation.SetActiveClip(AnimationClipType.WalkingUp); break;
                            case Orientation.Down:
                                animation.SetActiveClip(AnimationClipType.WalkingDown); break;
                            case Orientation.Left:
                                animation.SetActiveClip(AnimationClipType.WalkingLeft); break;
                            case Orientation.Right:
                                animation.SetActiveClip(AnimationClipType.WalkingRight); break;

                        }
                    }else if(movement.State == MovementState.Idle)
                    {
                        animation.Stop();
                    }
                }

            }
        }
    }
}
