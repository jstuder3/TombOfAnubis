using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Collections.Specialized.BitVector32;

namespace TombOfAnubis
{
    public class SpriteSystem : BaseSystem<Sprite>
    {
        public SpriteBatch SpriteBatch { get; set; }
        public Viewport Viewport { get; set; }
        public SpriteSystem(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            foreach(Sprite sprite in components)
            {
                Transform transform = sprite.Entity.GetComponent<Transform>();
                Vector2 worldPosition = transform.ToWorld().Position;
                Texture2D texture = sprite.Texture;
                Rectangle destinationRectangle = new Rectangle(
                    (int)worldPosition.X, 
                    (int)worldPosition.Y, 
                    (int)(sprite.SourceRectangle.Width * transform.Scale.X), 
                    (int)(sprite.SourceRectangle.Height * transform.Scale.Y)
                );
                if(CheckVisibility(destinationRectangle))
                {
                    SpriteBatch.Draw(texture, destinationRectangle, sprite.SourceRectangle, Color.White);
                }
            }
            SpriteBatch.End();
        }

        private bool CheckVisibility(Rectangle screenRectangle)
        {
            return ((screenRectangle.X > -screenRectangle.Width) &&
                (screenRectangle.Y > -screenRectangle.Height) &&
                (screenRectangle.X < Viewport.Width) &&
                (screenRectangle.Y < Viewport.Height));
        }
    }
}
