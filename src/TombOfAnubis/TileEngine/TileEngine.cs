using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    static class TileEngine
    {

        /// <summary>
        /// The position of the outside 0,0 corner of the map, in pixels.
        /// </summary>
        private static Vector2 mapOriginPosition;

        /// <summary>
        /// The position on the map in map coordinates that should be in the center of the viewport.
        /// </summary>
        private static Vector2 centeredMapPosition;

        /// <summary>
        /// The position on the map in map coordinates that should be in the center of the viewport.
        /// </summary>
        public static Vector2 CenteredMapPosition
        {
            get { return centeredMapPosition; }
            set
            {
                centeredMapPosition = value;
                mapOriginPosition = viewportCenter - centeredMapPosition;
            }
        }

        /// <summary>
        /// The viewport that the tile engine is rendering within.
        /// </summary>
        private static Viewport viewport;

        /// <summary>
        /// The viewport that the tile engine is rendering within.
        /// </summary>
        public static Viewport Viewport
        {
            get { return viewport; }
            set
            {
                viewport = value;
                viewportCenter = new Vector2(
                     viewport.Width / 2f,
                     viewport.Height / 2f);
            }
        }

        /// <summary>
        /// The center of the current viewport.
        /// </summary>
        private static Vector2 viewportCenter;

        /// <summary>
        /// The session whose content is rendered by the tile engine.
        /// </summary>
        private static Session session = null;

        /// <summary>
        /// Set the Session in use by the tile engine.
        /// </summary>
        /// <param name="newSession">The new session for the tile engine.</param>
        public static void SetSession(Session newSession)
        {
            // check the parameter
            if (newSession == null)
            {
                throw new ArgumentNullException("newSession");
            }

            session = newSession;

            mapOriginPosition = Vector2.Zero;
            centeredMapPosition = Vector2.Zero;
        }

        /// <summary>
        /// Draws the contents of the current session using the sprite batch system.
        /// </summary>
        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            DrawMapLayers(spriteBatch);

            foreach (Character character in session.Characters)
            {
                character.Draw(spriteBatch, mapOriginPosition);
            }
            spriteBatch.End();
        }
        public static void DrawMapLayers(SpriteBatch spriteBatch)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }


            Rectangle destinationRectangle =
                new Rectangle(0, 0, session.Map.TileSize.X, session.Map.TileSize.Y);

            for (int y = 0; y < session.Map.MapDimensions.Y; y++)
            {
                for (int x = 0; x < session.Map.MapDimensions.X; x++)
                {
                    destinationRectangle.X =
                        (int)mapOriginPosition.X + x * session.Map.TileSize.X;
                    destinationRectangle.Y =
                        (int)mapOriginPosition.Y + y * session.Map.TileSize.Y;

                    // If the tile is inside the screen
                    if (CheckVisibility(destinationRectangle))
                        {
                            Point mapPosition = new Point(x, y);

                            Rectangle sourceRectangle = session.Map.GetBaseLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(session.Map.Texture, destinationRectangle,
                                    sourceRectangle, Color.White);
                            }
                        
          
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the given rectangle is within the viewport.
        /// </summary>
        public static bool CheckVisibility(Rectangle screenRectangle)
        {
            return ((screenRectangle.X > - screenRectangle.Width) &&
                (screenRectangle.Y > - screenRectangle.Height) &&
                (screenRectangle.X <  viewport.Width) &&
                (screenRectangle.Y <  viewport.Height));
        }
    }


}
