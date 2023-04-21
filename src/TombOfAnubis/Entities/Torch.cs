using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TombOfAnubis
{
    public class Torch : Entity
    {
        public Torch(Vector2 position, Vector2 scale, Texture2D texture, Rectangle sourceRectangle)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, sourceRectangle, 1, Visibility.Game);
            AddComponent(sprite);
        }
    }
}
