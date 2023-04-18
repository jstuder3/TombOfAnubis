using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Sprite : Component
    {
        public int Layer { get; set; }

        public Texture2D Texture { get; set; }

        public Texture2D UndiscoveredTexture { get; set; }

        public Color Tint = Color.White;

        public Rectangle SourceRectangle { get; set; }

        public Sprite(Texture2D texture, int layer)
        {
            Texture = texture;
            SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Layer = layer;
            SpriteSystem.Register(this);
            SpriteSystem.SortComponents((x, y) => x.Layer.CompareTo(y.Layer));
        }
        public Sprite(Texture2D texture, Color tint, int layer) : this(texture, layer)
        {
            Tint = tint;
        }
        public Sprite(Texture2D texture, Rectangle sourceRectangle, int layer)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
            Layer = layer;
            SpriteSystem.Register(this);
            SpriteSystem.SortComponents((x, y) => x.Layer.CompareTo(y.Layer));
        }

        public override void Delete()
        {
            base.Delete();
            SpriteSystem.Deregister(this);
        }

    }
}
