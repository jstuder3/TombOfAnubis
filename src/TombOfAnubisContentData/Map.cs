using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TombOfAnubis
{
    public class Map : ContentObject
    {
        #region Map and tiles

        public string Name { get; set; }
        
        /// <summary>
        /// The dimensions of the map, in tiles.
        /// </summary>
        public Point MapDimensions { get; set; }

        /// <summary>
        /// The size of source tiles in the texture image.
        /// </summary>
        public Point SourceTileSize { get; set; }

        /// <summary>
        /// Scale of the source tiles to increase or decrease them for drawing.
        /// </summary>
        public Vector2 TileScale { get; set; }

        /// <summary>
        /// The actual size of the tiles (walls and floors) after applying scaling.
        /// </summary>
        [ContentSerializerIgnore]
        public Vector2 TileSize { get
            {
                return new Vector2(SourceTileSize.X * TileScale.X, SourceTileSize.Y * TileScale.Y);
            }
        }

        /// The number of tiles in a row of the map texture.
        /// </summary>
        /// <remarks>
        /// Used to determine the source rectangle from the map layer value.
        /// </remarks>
        [ContentSerializerIgnore]
        public int TilesPerRow { get; set; }
        /// <summary>
        /// The content name of the texture that contains the tiles for this map.
        /// </summary>
        public string TextureName { get; set; }
        /// <summary>
        /// The texture that contains the tiles for this map.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D Texture { get; set; }


        /// <summary>
        /// Spatial array for the ground tiles for this map.
        /// </summary>
        [ContentSerializerIgnore]
        public int[] BaseLayer { get; set; }

        /// <summary>
        /// Spatial array for the collision tiles for this map.
        /// </summary>
        public int[] CollisionLayer { get; set; }

        #endregion

        #region Entities

        /// <summary>
        /// Properties such as movement speeds are stored here
        /// </summary>
        public EntityProperties EntityProperties { get; set; }
        public List<EntityDescription> Characters { get; set; }
        public EntityDescription Anubis { get; set; }

        public List<EntityDescription> Artefacts { get; set; }

        public EntityDescription Altar { get; set; }

        public List<EntityDescription> Dispensers { get; set; }

        #endregion

        /// <summary>
        /// Retrieves the base layer value for the given map position.
        /// </summary>
        public int GetBaseLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= MapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= MapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return BaseLayer[mapPosition.Y * MapDimensions.X + mapPosition.X];
        }

        public int GetCollisionLayerValue(Point mapPosition)
        {
            // check the parameter
            if ((mapPosition.X < 0) || (mapPosition.X >= MapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= MapDimensions.Y))
            {
                throw new ArgumentOutOfRangeException("mapPosition");
            }

            return CollisionLayer[mapPosition.Y * MapDimensions.X + mapPosition.X];
        }

        public Rectangle GetBaseLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= MapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= MapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            int baseLayerValue = GetBaseLayerValue(mapPosition);
            if (baseLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (baseLayerValue % TilesPerRow) * SourceTileSize.X,
                (baseLayerValue / TilesPerRow) * SourceTileSize.Y,
                SourceTileSize.X, SourceTileSize.Y);
        }

        public Vector2 TileCoordinateToPosition(EntityDescription entityDescription)
        {
            Point tileCoordinate = entityDescription.SpawnTileCoordinate;
            Vector2 offset = entityDescription.Offset;
            Vector2 scale = entityDescription.Scale;
            Texture2D texture = entityDescription.Texture;
            var tilePos = new Vector2(tileCoordinate.X * TileSize.X,
                                       tileCoordinate.Y * TileSize.Y);
            var spriteCenter = tilePos + new Vector2(texture.Width * scale.X  / 2 - offset.X, texture.Height * scale.Y / 2 - offset.Y);
            var tileCenter = tilePos + new Vector2(TileSize.X / 2, TileSize.Y / 2);
            return tilePos + tileCenter - spriteCenter;
        }

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
                map.SourceTileSize = input.ReadObject<Point>();
                map.TileScale = input.ReadObject<Vector2>();
                //map.SpawnMapPosition = input.ReadObject<Point>();

                map.TextureName = input.ReadString();
                map.Texture = input.ContentManager.Load<Texture2D>(
                    Path.Combine(@"Textures\Maps",
                    map.TextureName));
                map.TilesPerRow = map.Texture.Width / map.SourceTileSize.X;
                map.CollisionLayer = input.ReadObject<int[]>();
                map.BaseLayer = input.ReadObject<int[]>();
                map.EntityProperties = input.ReadObject<EntityProperties>();
                map.Characters = input.ReadObject<List<EntityDescription>>();
                foreach(EntityDescription ed in map.Characters)
                {
                    ed.Texture = input.ContentManager.Load<Texture2D>(
                        Path.Combine(@"Textures\Characters",
                        ed.SpriteTextureName));
                }
                map.Anubis = input.ReadObject<EntityDescription>();
                map.Anubis.Texture = input.ContentManager.Load<Texture2D>(
                        Path.Combine(@"Textures\Characters",
                        map.Anubis.SpriteTextureName));
                map.Artefacts = input.ReadObject<List<EntityDescription>>();
                foreach (EntityDescription ed in map.Artefacts)
                {
                    ed.Texture = input.ContentManager.Load<Texture2D>(
                        Path.Combine(@"Textures\Objects\Artefacts",
                        ed.SpriteTextureName));
                }
                map.Altar = input.ReadObject<EntityDescription>();
                map.Altar.Texture = input.ContentManager.Load<Texture2D>(
                        Path.Combine(@"Textures\Objects\Altar",
                        map.Altar.SpriteTextureName));
                map.Dispensers = input.ReadObject<List<EntityDescription>>();
                foreach (EntityDescription ed in map.Dispensers)
                {
                    ed.Texture = input.ContentManager.Load<Texture2D>(
                        Path.Combine(@"Textures\Objects\Dispensers",
                        ed.SpriteTextureName));
                }

                return map;
            }
        }
    }
    
}

