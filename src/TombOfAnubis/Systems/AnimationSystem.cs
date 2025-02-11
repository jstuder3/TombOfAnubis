﻿using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class AnimationSystem : BaseSystem<Animation>
    {
        public override void Update(GameTime gameTime)
        {
            float totalTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            foreach (Animation animation in GetComponents())
            {
                if (animation.Entity == null) continue;
                Sprite sprite = animation.Entity.GetComponent<Sprite>();
                AnimationClip activeClip = animation.ActiveClip;

                if(activeClip != null && sprite != null)
                {
                    if(sprite.SourceRectangle.Y == activeClip.SourceRectangle.Y && !animation.IsStopped)
                    {
                        int frameIdx = (int)(totalTime / activeClip.FrameDuration) % activeClip.NumberOfFrames;
                        sprite.SourceRectangle = new Rectangle(
                            frameIdx * activeClip.FrameSize.X,
                            activeClip.SourceRectangle.Y,
                            activeClip.SourceRectangle.Width,
                            activeClip.SourceRectangle.Height
                           );
                    }
                    else
                    {
                        sprite.SourceRectangle = activeClip.SourceRectangle;
                    }

                }
                else if( sprite  != null )
                {
                    sprite.SourceRectangle = animation.AnimationClips[0].SourceRectangle;
                }
            }
        }
    }
}
