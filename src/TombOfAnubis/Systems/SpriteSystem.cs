﻿using Microsoft.Xna.Framework;
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
            var components = GetComponents();
            foreach (Sprite sprite in components)
            {
                Entity entity = sprite.Entity;
                Vector2 entitySize = entity.DrawingSize();
                Texture2D texture = sprite.Texture;
                Vector2 position = entity.DrawingPosition();
                Rectangle sourceRectangle = sprite.SourceRectangle;
                Color color = sprite.Tint * sprite.Alpha;
                float rotation = 0f;
                Vector2 origin = Vector2.Zero;
                Vector2 scale = entitySize / new Vector2(sourceRectangle.Width, sourceRectangle.Height);

                Rectangle destinationRectangle = new Rectangle(
                    (int)position.X,
                    (int)position.Y,
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
                            SpriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None, 0f);
                        }
                    }
                    else
                    {
                        SpriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None, 0f);
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
