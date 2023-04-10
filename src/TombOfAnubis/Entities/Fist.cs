using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO.Compression;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public class Fist : Entity
    {
        private Vector2 startPosition;
        private float velocity;
        private float range;

        public Fist(Vector2 position, Vector2 scale, Texture2D texture, List<AnimationClip> animationClips, Vector2 forwardVector)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite;
            if (animationClips != null)
            {
                Animation animation = new Animation(animationClips);
                AddComponent(animation);

                // select sprite based on forward vector
                if(forwardVector.X == 1 && forwardVector.Y == 0)
                {
                    animation.SetActiveClip(AnimationClipType.WalkingRight);
                }
                else if(forwardVector.X == -1 && forwardVector.Y == 0)
                {
                    animation.SetActiveClip(AnimationClipType.WalkingLeft);
                }
                else if (forwardVector.X == 0 && forwardVector.Y == 1)
                {
                    animation.SetActiveClip(AnimationClipType.WalkingDown);
                }
                else if (forwardVector.X == 0 && forwardVector.Y == -1)
                {
                    animation.SetActiveClip(AnimationClipType.WalkingUp);
                }
                else
                {
                    animation.SetActiveClip(AnimationClipType.WalkingRight);
                }


                sprite = new Sprite(texture, animation.DefaultSourceRectangle, 2);
            }
            else
            {
                sprite = new Sprite(texture, 2);
            }
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);
        }

    }
}
