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
        None,
        WalkingLeft,
        WalkingRight,
        WalkingUp,
        WalkingDown,
        Idle,
        ObjectActive,
        ObjectInactive,
        Pressed,
        NotPressed,
        VFX_01
    }

    public class AnimationClip
    {
        public AnimationClipType Type { get; set; }
        public int NumberOfFrames { get; set; }
        public float FrameDuration { get; set; }
        public Point FrameSize { get; set; }

        [ContentSerializerIgnore]
        public Rectangle SourceRectangle { get; set; }
    }
}
