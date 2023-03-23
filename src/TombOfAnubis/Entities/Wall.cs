using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Wall : Entity
    {
        public Wall(Vector2 position, Vector2 scale, Texture2D texture, Rectangle sourceRectangle)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, sourceRectangle, 0);
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);
        }
    }
}
