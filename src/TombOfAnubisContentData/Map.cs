using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using TombOfAnubisContentData;
using static System.Formats.Asn1.AsnWriter;
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
        [ContentSerializerIgnore]
        public Vector2 MapSize {
            get
            {
                return new Vector2(MapDimensions.X * TileSize.X, MapDimensions.Y * TileSize.Y);
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
        /// The texture of undiscovered tiles.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D UndiscoveredTexture { get; set; }

        public int NumberOfFloorTiles { get; set; }

        [ContentSerializerIgnore]
        private Random floorTileRandom = new Random();
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

        //public EntityDescription Fist { get; set; }

        public List<EntityDescription> Traps { get; set; }

        public List<EntityDescription> Buttons { get; set; }

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
            else if (baseLayerValue > 15)
            {
                // Pick a random floor tile (Row 1)
                int index = floorTileRandom.Next(NumberOfFloorTiles);
                return new Rectangle(
                    index * SourceTileSize.X,
                    SourceTileSize.Y,
                    SourceTileSize.X,
                    SourceTileSize.Y
                    );
            }
            else
            {
                // Chooose the wall tile texture based on its neighbours (Row 0)
                return new Rectangle(
                (baseLayerValue % TilesPerRow) * SourceTileSize.X,
                0,
                SourceTileSize.X, SourceTileSize.Y);
            }
        }

        public Vector2 CreateEntityTileCenteredPosition(EntityDescription entityDescription)
        {
            Point tileCoordinates = entityDescription.SpawnTileCoordinate;
            Vector2 offset = entityDescription.Offset;
            Vector2 scale = entityDescription.Scale;

            AnimationClip clip = entityDescription.Animation?[0];
            Rectangle sourceRectangle;
            if(clip != null)
            {
                sourceRectangle = clip.SourceRectangle;
            }
            else
            {
                sourceRectangle = new Rectangle(0, 0, entityDescription.Texture.Width, entityDescription.Texture.Height);
            }

            var tilePos = TileCoordinateToPosition(tileCoordinates);
            var spriteCenter = tilePos + new Vector2(sourceRectangle.Width * scale.X  / 2 - offset.X, sourceRectangle.Height * scale.Y / 2 - offset.Y);
            var tileCenter = tilePos + new Vector2(TileSize.X / 2, TileSize.Y / 2);
            return tilePos + tileCenter - spriteCenter;
        }

        public Vector2 CreateEntityTileCenteredPositionSpriteless(EntityDescription entityDescription)
        {
            Point tileCoordinates = entityDescription.SpawnTileCoordinate;
            Vector2 offset = entityDescription.Offset;

            var tilePos = TileCoordinateToPosition(tileCoordinates);
            var tileCenter = new Vector2(TileSize.X / 2, TileSize.Y / 2);
            return tilePos + tileCenter + offset;
        }

        /// <summary>
        /// Takes a position on the tile map and returns the coordinates of the tile under this position.
        /// </summary>
        public Point PositionToTileCoordinate(Vector2 position)
        {
            return new Point((int)(position.X / TileSize.X), (int)(position.Y / TileSize.Y));
        }
        /// <summary>
        /// Takes a tile coordinate and returns the position of the top left corner of this tile.
        /// </summary>
        public Vector2 TileCoordinateToPosition(Point tileCoordinate)
        {
            return new Vector2(tileCoordinate.X * TileSize.X, tileCoordinate.Y * TileSize.Y);
        }
        /// <summary>
        /// Takes a tile coordinate and returns the position of the center of this tile
        /// </summary>
        public Vector2 TileCoordinateToTileCenterPosition(Point tileCoordinate)
        {
            return new Vector2(tileCoordinate.X * TileSize.X + TileSize.X / 2f, tileCoordinate.Y * TileSize.Y + TileSize.Y / 2f);
        }

        public bool ValidTileCoordinates(Point coords)
        {
            return coords.X >= 0 && coords.Y >= 0 && coords.X < MapDimensions.X && coords.Y < MapDimensions.Y;
        }

        /// <summary>
        /// Read a Map object from the content pipeline.
        /// </summary>
        public class MapReader : ContentTypeReader<Map>
        {
            protected override Map Read(ContentReader input, Map existingInstance)
            {
                // Map data
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
                map.NumberOfFloorTiles = input.ReadInt32();
                map.CollisionLayer = input.ReadObject<int[]>();
                map.BaseLayer = input.ReadObject<int[]>();
                map.EntityProperties = input.ReadObject<EntityProperties>();

                // Characters
                map.Characters = input.ReadObject<List<EntityDescription>>();
                foreach(EntityDescription ed in map.Characters)
                {
                    ed.Load(input.ContentManager, @"Textures\Characters");
                }

                // Anubis
                map.Anubis = input.ReadObject<EntityDescription>();
                map.Anubis.Load(input.ContentManager, @"Textures\Characters");
                map.Anubis.Texture = input.ContentManager.Load<Texture2D>(
                        Path.Combine(@"Textures\Characters",
                        map.Anubis.SpriteTextureName));

                // Artefacts
                map.Artefacts = input.ReadObject<List<EntityDescription>>();
                foreach (EntityDescription ed in map.Artefacts)
                {
                    ed.Load(input.ContentManager, @"Textures\Objects\Artefacts");

                }

                // Altar
                map.Altar = input.ReadObject<EntityDescription>();
                map.Altar.Load(input.ContentManager, @"Textures\Objects\Altar");

                // Dispensers
                map.Dispensers = input.ReadObject<List<EntityDescription>>();
                foreach (EntityDescription ed in map.Dispensers)
                {
                    ed.Load(input.ContentManager, @"Textures\Objects\Dispensers");
                }

                // Traps
                map.Traps = input.ReadObject<List<EntityDescription>>();
                foreach(EntityDescription ed in map.Traps)
                {
                    ed.Load(input.ContentManager, @"Textures\Objects\Traps");
                }

                // Buttons
                map.Buttons = input.ReadObject<List<EntityDescription>>();
                foreach (EntityDescription ed in map.Buttons)
                {
                    ed.Load(input.ContentManager, @"Textures\Objects\Buttons");
                }

                // Fist
                //map.Fist = input.ReadObject<EntityDescription>();
                //map.Fist.Load(input.ContentManager, @"Textures\Objects\Items");


                return map;
            }
        }
    }
    
}

