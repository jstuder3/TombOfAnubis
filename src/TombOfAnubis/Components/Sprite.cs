using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Sprite : Component
    {
        public int Layer { get; set; }

        public Texture2D Texture { get; set; }

        public Rectangle SourceRectangle { get; set; }

        public Sprite(Texture2D texture, int layer) {
            Texture = texture;
            SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Layer = layer;
            SpriteSystem.Register(this);
            SpriteSystem.SortComponents((x, y) => x.Layer.CompareTo(y.Layer));
        }
        public Sprite(Texture2D texture, Rectangle sourceRectangle, int layer)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
            Layer = layer;
            SpriteSystem.Register(this);
            SpriteSystem.SortComponents((x, y) => x.Layer.CompareTo(y.Layer));
        }

        public override void DeleteComponent()
        {
            base.DeleteComponent();
            SpriteSystem.Deregister(this);
        }

    }
}
