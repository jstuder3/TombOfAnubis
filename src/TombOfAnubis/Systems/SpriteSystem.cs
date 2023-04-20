using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
            //SpriteBatch.Begin();
            var components = GetComponents();
            foreach (Sprite sprite in components)
            {
                Entity entity = sprite.Entity;
                Transform transform = entity.GetComponent<Transform>();
                Vector2 entitySize = entity.Size(Session.GetInstance().Visibility);
                Vector2 worldPosition = transform.ToWorld().Position;
                Texture2D texture = sprite.Texture;
                Rectangle destinationRectangle = new Rectangle(
                    (int)worldPosition.X,
                    (int)worldPosition.Y,
                    (int)entitySize.X,
                    (int)entitySize.Y
                ); 
                if (CheckVisibility(destinationRectangle))
                {
                    if (Session.GetInstance().Visibility == Visibility.Minimap)
                    {
                        Discovery discovery = entity.GetComponent<Discovery>();
                        if (discovery == null || discovery.Discovered) 
                        {
                            SpriteBatch.Draw(texture, destinationRectangle, sprite.SourceRectangle, sprite.Tint * sprite.Alpha);
                        }
                    }
                    else
                    {
                        SpriteBatch.Draw(texture, destinationRectangle, sprite.SourceRectangle, sprite.Tint * sprite.Alpha);   
                    }
                }
            }
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
