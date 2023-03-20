using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Floor : Entity
    {
        public Floor(Vector2 position, Texture2D texture, Rectangle sourceRectangle)
        {
            Transform transform = new Transform(position);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, sourceRectangle, 0);
            AddComponent(sprite);
        }
    }
}
