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

        private static Session session = null;
        /// <summary>
        /// The position of the outside 0,0 corner of the map, in pixels.
        /// </summary>
        private static Vector2 mapOriginPosition;

        private static Vector2 centeredMapPosition;
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
        /// Set the map in use by the tile engine.
        /// </summary>
        /// <param name="map">The new map for the tile engine.</param>
        public static void SetSession(Session newSession)
        {
            // check the parameter
            if (newSession == null)
            {
                throw new ArgumentNullException("newSession");
            }

            // assign the new map
            session = newSession;

            // reset the map origin, which will be recalculated on the first update
            mapOriginPosition = Vector2.Zero;
            centeredMapPosition = Vector2.Zero;
            //mapOriginPosition = new Vector2(-32, 0);

            //// move the party to its initial position

            //partyLeaderPosition.TilePosition = map.SpawnMapPosition;
            //partyLeaderPosition.TileOffset = Vector2.Zero;
            //partyLeaderPosition.Direction = Direction.South;

        }

        /// <summary>
        /// Update the tile engine.
        /// </summary>
        public static void Update(GameTime gameTime, Vector2 focusedPlayerLocation)
        {
            // if there is no auto-movement, handle user controls
            Vector2 userMovement = Vector2.Zero;

            //userMovement = UpdateUserMovement(gameTime);
            // calculate the desired position
            //if (userMovement != Vector2.Zero)
            //{
            //    Point desiredTilePosition = partyLeaderPosition.TilePosition;
            //    Vector2 desiredTileOffset = partyLeaderPosition.TileOffset;
            //    PlayerPosition.CalculateMovement(
            //        Vector2.Multiply(userMovement, 15f),
            //        ref desiredTilePosition, ref desiredTileOffset);
            //    // check for collisions or encounters in the new tile
            //    if ((partyLeaderPosition.TilePosition != desiredTilePosition) &&
            //        !MoveIntoTile(desiredTilePosition))
            //    {
            //        userMovement = Vector2.Zero;
            //    }
            //}
            //}

            // move the party

            //Point oldPartyLeaderTilePosition = partyLeaderPosition.TilePosition;
            //partyLeaderPosition.Move(autoMovement + userMovement);


            // adjust the map origin so that the party is at the center of the viewport

            //mapOriginPosition += viewportCenter - (partyLeaderPosition.ScreenPosition + Session.Party.Players[0].MapSprite.SourceOffset);

            // make sure the boundaries of the map are never inside the viewport

            //mapOriginPosition.X = MathHelper.Min(mapOriginPosition.X, viewport.X);
            //mapOriginPosition.Y = MathHelper.Min(mapOriginPosition.Y, viewport.Y);
            //mapOriginPosition.X += MathHelper.Max(
            //    (viewport.X + viewport.Width) -
            //    (mapOriginPosition.X + map.MapDimensions.X * map.TileSize.X), 0f);
            //mapOriginPosition.Y += MathHelper.Max(
            //    (viewport.Y + viewport.Height - Hud.HudHeight) -
            //(mapOriginPosition.Y + map.MapDimensions.Y * map.TileSize.Y), 0f);
        }
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
