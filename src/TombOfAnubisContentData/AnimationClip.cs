using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubisContentData
{
    public enum AnimationClipType
    {
        // Default
        None,
        // Character movement state
        WalkingLeft,
        WalkingRight,
        WalkingUp,
        WalkingDown,
        Idle,
        ObjectActive,
        ObjectInactive,
        // Trap button
        Pressed,
        NotPressed,
        // Character death
        Dead,
        // Anubis-Fist collision
        VFX_01,
        // MenuEntry scroll state
        InactiveEntry,
        TransitionEntry,
        ActiveEntry,
    }

    public class AnimationClip
    {

        public AnimationClipType Type { get; set; }
        public int NumberOfFrames { get; set; }
        public float FrameDuration { get; set; }
        public Point FrameSize { get; set; }

        [ContentSerializerIgnore]
        public Rectangle SourceRectangle { get; set; }

        public AnimationClip() { }

        public AnimationClip(AnimationClipType type, int numberOfFrames, float frameDuration, Point frameSize)
        {
            Type = type;
            NumberOfFrames = numberOfFrames;
            FrameDuration = frameDuration;
            FrameSize = frameSize;
        }

        public float GetTotalClipDuration()
        {
            return FrameDuration * NumberOfFrames;
        }

    }
}
