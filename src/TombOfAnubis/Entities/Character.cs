﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public class Character : Entity
    {
        public Character(int playerID, Vector2 position, Vector2 scale, Texture2D texture, int maxMovementSpeed, List<AnimationClip> animationClips)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite;
            if(animationClips != null)
            {
                Animation animation = new Animation(animationClips);
                AddComponent(animation);

                animation.SetActiveClip(AnimationClipType.WalkingRight);

                sprite = new Sprite(texture, animation.DefaultSourceRectangle, 2);
            }
            else
            {
                sprite = new Sprite(texture, 2);
            }
            AddComponent(sprite);

            Player player = new Player(playerID);
            AddComponent(player);

            Input input = new Input();
            AddComponent(input);

            Movement movement = new Movement(maxMovementSpeed);
            AddComponent(movement);

            Inventory inventory = new Inventory(1, 3);
            AddComponent(inventory);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);

        }
    }
}
