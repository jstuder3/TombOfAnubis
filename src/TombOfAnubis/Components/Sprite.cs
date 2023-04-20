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

        public float Alpha = 1f;

        public Sprite(int layer, Visibility visibility)
        {
            Layer = layer;
            Visibility = visibility;
            SpriteSystem.Register(this);
            SpriteSystem.SortComponents((x, y) => x.Layer.CompareTo(y.Layer));
        }

        public Sprite(Texture2D texture, Rectangle sourceRectangle, int layer, Visibility visibility) : this(layer, visibility)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
        }
        public Sprite(Texture2D texture, int layer, Visibility visibility) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height), layer, visibility)
        {
        }
        public Sprite(Texture2D texture, Color tint, int layer, Visibility visibility) : this(texture, layer, visibility)
        {
            Tint = tint;
        }
        
        public override void Delete()
        {
            base.Delete();
            SpriteSystem.Deregister(this);
        }
    }
}
