using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public class VFX : Entity
    {
        public VFX(Vector2 position, Vector2 scale, Texture2D texture, int layer)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, layer);
            AddComponent(sprite);

        }

        public VFX(Vector2 position, Vector2 scale, Texture2D texture, List<AnimationClip> animationClips, int layer, AnimationClipType clipToPlay)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);


            if (animationClips != null)
            {
                Animation animation = new Animation(animationClips);
                AddComponent(animation);

                animation.SetActiveClip(clipToPlay);

                Sprite sprite = new Sprite(texture, animation.ActiveClip.SourceRectangle, layer);
                AddComponent(sprite);
            }

        }

    }
}
