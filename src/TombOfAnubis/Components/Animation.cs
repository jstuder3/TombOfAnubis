using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public class Animation : Component
    {
        public AnimationClip ActiveClip { get; set; }
        public List<AnimationClip> AnimationClips { get; set; }

        public Rectangle DefaultSourceRectangle { get; set; }

        public Animation(List<AnimationClip> animationClips, Visibility visibility)
        {
            AnimationClips = animationClips;
            Visibility = visibility;
            SetActiveClip(AnimationClipType.Idle);
            DefaultSourceRectangle = animationClips[0].SourceRectangle;
            ComputeAnimationClipRectangles();
            AnimationSystem.Register(this);
        }

        public override void Delete()
        {
            AnimationSystem.Deregister(this);
            base.Delete();
        }

        public void ComputeAnimationClipRectangles()
        {
            int startPosition = 0;
            for (int i = 0; i < AnimationClips.Count; i++)
            {
                AnimationClips[i].SourceRectangle = new Rectangle(0, startPosition, AnimationClips[i].FrameSize.X, AnimationClips[i].FrameSize.Y);
                startPosition += AnimationClips[i].FrameSize.Y;
            }
        }

        public float GetCurrentClipDuration()
        {
            return ActiveClip.GetTotalClipDuration();
        }

        /// <summary>
        /// Sets the active clip. If the clip of type clipType was not found, false is returned.
        /// </summary>
        public bool SetActiveClip(AnimationClipType clipType)
        {
            ActiveClip = null;
            for (int i = 0; i < AnimationClips.Count; i++)
            {
                AnimationClip clip = AnimationClips[i];
                if(clip.Type == clipType)
                {
                    ActiveClip = clip;
                }
            }
            return ActiveClip != null;
        }
    }
}
