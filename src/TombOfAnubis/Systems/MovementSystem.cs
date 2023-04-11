﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public class MovementSystem : BaseSystem<Movement>
    {
        public override void Update(GameTime deltaTime)
        {
            foreach (Movement movement in components)
            {
                Entity entity = movement.Entity;
                Animation animation = entity.GetComponent<Animation>();
                if (animation != null)
                {
                    if(movement.State == MovementState.Walking)
                    {
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
                    }else if(movement.State == MovementState.Idle) //note: we need an idle-state for every orientation, otherwise the player "snaps" back to the default animation whenever you stop pressing buttons, so I disabled this for now
                    {
                        //animation.SetActiveClip(AnimationClipType.Idle);
                    }
                }

            }
        }
    }
}
