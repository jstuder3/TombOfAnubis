using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Floor : Entity
    {
        public Floor(Vector2 position, Vector2 scale, Texture2D texture, Rectangle sourceRectangle)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, sourceRectangle, 0);
            AddComponent(sprite);
        }
    }
}
