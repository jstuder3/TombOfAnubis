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
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, layer, Visibility.Game);
            AddComponent(sprite);

        }

        public VFX(Vector2 position, Vector2 scale, Texture2D texture, List<AnimationClip> animationClips, int layer, AnimationClipType clipToPlay)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);


            if (animationClips != null)
            {
                Animation animation = new Animation(animationClips, Visibility.Game);
                AddComponent(animation);

                animation.SetActiveClip(clipToPlay);

                Sprite sprite = new Sprite(texture, animation.ActiveClip.SourceRectangle, layer, Visibility.Game);
                AddComponent(sprite);
            }

        }

    }
}
