﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public class Anubis : Entity
    {
        public Anubis(Vector2 position, Vector2 scale, Texture2D texture, List<AnimationClip> animationClips, int maxMovementSpeed, Map map)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite;
            if (animationClips != null)
            {
                Animation animation = new Animation(animationClips);
                AddComponent(animation);

                animation.SetActiveClip(AnimationClipType.Idle);

                sprite = new Sprite(texture, animation.DefaultSourceRectangle, 2);
            }
            else
            {
                sprite = new Sprite(texture, 2);
            }
            AddComponent(sprite);

            Movement movement = new Movement(maxMovementSpeed);
            AddComponent(movement);

            AI ai = new AI(map);
            AddComponent(ai);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);
        }
    }
}
