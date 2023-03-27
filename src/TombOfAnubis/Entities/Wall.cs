using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
