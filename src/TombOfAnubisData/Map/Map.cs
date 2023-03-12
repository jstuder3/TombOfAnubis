using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace TombOfAnubis
{
    public class Map : ContentObject
    {

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The dimensions of the map, in tiles.
        /// </summary>
        private Point mapDimensions;

        /// <summary>
        /// The dimensions of the map, in tiles.
        /// </summary>
        public Point MapDimensions
        {
            get { return mapDimensions; }
            set { mapDimensions = value; }
        }

        /// <summary>
        /// The size of the tiles in this map, in pixels.
        /// </summary>
        private Point tileSize;

        /// <summary>
        /// The size of the tiles in this map, in pixels.
        /// </summary>
        public Point TileSize
        {
            get { return tileSize; }
            set { tileSize = value; }
        }
        // <summary>
        /// The number of tiles in a row of the map texture.
        /// </summary>
        /// <remarks>
        /// Used to determine the source rectangle from the map layer value.
        /// </remarks>
        private int tilesPerRow;

        /// <summary>
        /// The number of tiles in a row of the map texture.
        /// </summary>
        /// <remarks>
        /// Used to determine the source rectangle from the map layer value.
        /// </remarks>
        [ContentSerializerIgnore]
        public int TilesPerRow
        {
            get { return tilesPerRow; }
        }

        /// <summary>
        /// A valid spawn position for this map. 
        /// </summary>
        private Point spawnMapPosition;

        /// <summary>
        /// A valid spawn position for this map. 
        /// </summary>
        public Point SpawnMapPosition
        {
            get { return spawnMapPosition; }
            set { spawnMapPosition = value; }
        }

        /// <summary>
        /// The content name of the texture that contains the tiles for this map.
        /// </summary>
        private string textureName;

        /// <summary>
        /// The content name of the texture that contains the tiles for this map.
        /// </summary>
        public string TextureName
        {
            get { return textureName; }
            set { textureName = value; }
        }

        /// <summary>
        /// The texture that contains the tiles for this map.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// The texture that contains the tiles for this map.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get { return texture; }
        }

        /// <summary>
        /// Spatial array for the ground tiles for this map.
        /// </summary>
        private int[] baseLayer;

        /// <summary>
        /// Spatial array for the ground tiles for this map.
        /// </summary>
        public int[] BaseLayer
        {
            get { return baseLayer; }
            set { baseLayer = value; }
        }

        /// <summary>
        /// Retrieves the base layer value for the given map position.
        /// </summary>
        public int GetBaseLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return baseLayer[mapPosition.Y * mapDimensions.X + mapPosition.X];
        }

        public Rectangle GetBaseLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= mapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= mapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int baseLayerValue = GetBaseLayerValue(mapPosition);
            if (baseLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (baseLayerValue % tilesPerRow) * tileSize.X,
                (baseLayerValue / tilesPerRow) * tileSize.Y,
                tileSize.X, tileSize.Y);
        }

        //public void Preprocess(ContentManager content)
        //{
        //    texture = content.Load<Texture2D>(
        //            System.IO.Path.Combine(@"Textures\Maps",
        //            TextureName));
        //}

        /// <summary>
        /// Read a Map object from the content pipeline.
        /// </summary>
        public class MapReader : ContentTypeReader<Map>
        {
            protected override Map Read(ContentReader input, Map existingInstance)
            {
                Map map = existingInstance;
                if (map == null)
                {
                    map = new Map();
                }

                map.AssetName = input.AssetName;

                map.Name = input.ReadString();
                map.MapDimensions = input.ReadObject<Point>();
                map.TileSize = input.ReadObject<Point>();
                map.SpawnMapPosition = input.ReadObject<Point>();

                map.TextureName = input.ReadString();
                map.texture = input.ContentManager.Load<Texture2D>(
                    System.IO.Path.Combine(@"Textures\Maps",
                    map.TextureName));
                map.tilesPerRow = map.texture.Width / map.TileSize.X;

               

                return map;
            }
        }
    }
    
}

