using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Floor : Entity
    {
        public Floor(Vector2 position, Vector2 scale, Texture2D texture, Texture2D undiscoveredTexture, Rectangle sourceRectangle)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, sourceRectangle, 0);
            sprite.UndiscoveredTexture = undiscoveredTexture;
            AddComponent(sprite);

            Discovery discovery = new Discovery();
            AddComponent(discovery);
        }
    }
}
