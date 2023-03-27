using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            foreach (Sprite sprite in components)
            {
                Entity entity = sprite.Entity;
                Transform transform = entity.GetComponent<Transform>();
                Vector2 entitySize = entity.Size();
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
