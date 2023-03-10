using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class TileEngine
    {
        /// <summary>
        /// The map being used by the tile engine.
        /// </summary>
        private static Map map = null;

        /// <summary>
        /// The map being used by the tile engine.
        /// </summary>
        public static Map Map
        {
            get { return map; }
        }

        /// <summary>
        /// The position of the outside 0,0 corner of the map, in pixels.
        /// </summary>
        private static Vector2 mapOriginPosition;


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
                    viewport.X + viewport.Width / 2f,
                    viewport.Y + viewport.Height / 2f);
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
        public static void SetMap(Map newMap)
        {
            // check the parameter
            if (newMap == null)
            {
                throw new ArgumentNullException("newMap");
            }

            // assign the new map
            map = newMap;

            // reset the map origin, which will be recalculated on the first update
            mapOriginPosition = Vector2.Zero;

            //// move the party to its initial position

            //partyLeaderPosition.TilePosition = map.SpawnMapPosition;
            //partyLeaderPosition.TileOffset = Vector2.Zero;
            //partyLeaderPosition.Direction = Direction.South;

        }
        public static void DrawLayers(SpriteBatch spriteBatch, bool drawBase,
            bool drawFringe, bool drawObject)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }
            if (!drawBase && !drawFringe && !drawObject)
            {
                return;
            }

            Rectangle destinationRectangle =
                new Rectangle(0, 0, map.TileSize.X, map.TileSize.Y);

            for (int y = 0; y < map.MapDimensions.Y; y++)
            {
                for (int x = 0; x < map.MapDimensions.X; x++)
                {
                    destinationRectangle.X =
                        (int)mapOriginPosition.X + x * map.TileSize.X;
                    destinationRectangle.Y =
                        (int)mapOriginPosition.Y + y * map.TileSize.Y;

                    // If the tile is inside the screen
                    if (CheckVisibility(destinationRectangle))
                    {
                        Point mapPosition = new Point(x, y);
                        if (drawBase)
                        {
                            Rectangle sourceRectangle =
                                map.GetBaseLayerSourceRectangle(mapPosition);
                            if (sourceRectangle != Rectangle.Empty)
                            {
                                spriteBatch.Draw(map.Texture, destinationRectangle,
                                    sourceRectangle, Color.White);
                            }
                        }
                        //if (drawFringe)
                        //{
                        //    Rectangle sourceRectangle =
                        //        map.GetFringeLayerSourceRectangle(mapPosition);
                        //    if (sourceRectangle != Rectangle.Empty)
                        //    {
                        //        spriteBatch.Draw(map.Texture, destinationRectangle,
                        //            sourceRectangle, Color.White);
                        //    }
                        //}
                        //if (drawObject)
                        //{
                        //    Rectangle sourceRectangle =
                        //        map.GetObjectLayerSourceRectangle(mapPosition);
                        //    if (sourceRectangle != Rectangle.Empty)
                        //    {
                        //        spriteBatch.Draw(map.Texture, destinationRectangle,
                        //            sourceRectangle, Color.White);
                        //    }
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the given rectangle is within the viewport.
        /// </summary>
        public static bool CheckVisibility(Rectangle screenRectangle)
        {
            return ((screenRectangle.X > viewport.X - screenRectangle.Width) &&
                (screenRectangle.Y > viewport.Y - screenRectangle.Height) &&
                (screenRectangle.X < viewport.X + viewport.Width) &&
                (screenRectangle.Y < viewport.Y + viewport.Height));
        }
    }


}
