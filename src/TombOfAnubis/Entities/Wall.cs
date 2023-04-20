﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Wall : Entity
    {
        public Wall(Vector2 position, Vector2 scale, Texture2D texture, Texture2D undiscoveredTexture, Rectangle sourceRectangle)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, sourceRectangle, 0, Visibility.Game);
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(position, Size(Visibility.Game));
            AddComponent(collider);

            Discovery discovery = new Discovery();
            AddComponent(discovery);
        }
    }
}
