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
            //SpriteBatch.Begin();
            foreach (Sprite sprite in components)
            {
                Entity entity = sprite.Entity;
                Transform transform = entity.GetComponent<Transform>();
                Discovery discovery = entity.GetComponent<Discovery>();
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
                    if(discovery != null && !discovery.Discovered && sprite.UndiscoveredTexture != null)
                    {
                        SpriteBatch.Draw(sprite.UndiscoveredTexture, destinationRectangle, sprite.Tint);
                    }
                    else
                    {
                        Point entityTileCoord = Session.GetInstance().Map.PositionToTileCoordinate(transform.Position);
                        Entity[,] mapTiles = Session.GetInstance().MapTiles;
                        int width = mapTiles.GetLength(0);
                        int height = mapTiles.GetLength(1);
                        if (entityTileCoord.X >= 0 && entityTileCoord.X < width && entityTileCoord.Y >= 0 && entityTileCoord.Y < height) //check that we're in bounds
                        {
                            bool onDiscoveredTile = mapTiles[entityTileCoord.X, entityTileCoord.Y].GetComponent<Discovery>().Discovered;
                            if (onDiscoveredTile)
                            {
                                SpriteBatch.Draw(texture, destinationRectangle, sprite.SourceRectangle, sprite.Tint);
                            }
                        }
                    }
                }
            }
            //SpriteBatch.End();
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
