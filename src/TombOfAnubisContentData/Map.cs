using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;


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

        [ContentSerializerIgnore]
        public Texture2D TorchTexture { get; set; }

        [ContentSerializerIgnore]
        public List<Rectangle> TorchSourceRectangles { get; set; }

        /// <summary>
        /// The texture of undiscovered tiles.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D UndiscoveredTexture { get; set; }

        public int NumberOfFloorTiles { get; set; }

        public Vector2 WorldScale { get; set; }

        public float TorchProbability { get; set; }

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
        [ContentSerializerIgnore]
        public int[] CollisionLayer { get; set; }

        #endregion

        #region Entities

        /// <summary>
        /// Properties such as movement speeds are stored here
        /// </summary>
        public EntityProperties EntityProperties { get; set; }

        public List<MapBlockDescription> MapBlockDescriptions { get; set; }

        [ContentSerializerIgnore]
        public List<MapBlock> MapBlocks { get; set; }

        [ContentSerializerIgnore]
        public List<EntityDescription> Characters { get; set; }

        [ContentSerializerIgnore]
        public EntityDescription Anubis { get; set; }

        [ContentSerializerIgnore]
        public List<EntityDescription> Artefacts { get; set; }

        [ContentSerializerIgnore]
        public EntityDescription Altar { get; set; }

        [ContentSerializerIgnore]
        public List<EntityDescription> Dispensers { get; set; }

        [ContentSerializerIgnore]
        public EntityDescription Fist { get; set; }

        [ContentSerializerIgnore]
        public List<EntityDescription> Traps { get; set; }

        [ContentSerializerIgnore]
        public List<EntityDescription> Buttons { get; set; }

        #endregion

        public void Reset()
        {
            BaseLayer = null;
            CollisionLayer = null;
            foreach(MapBlockDescription desc in MapBlockDescriptions)
            {
                desc.Reset();
            }
            MapBlocks = null;
            Characters = null;
            Anubis = null;
            Artefacts = null;
            Altar = null;
            Dispensers = null;
            Fist = null;
            Traps = null;
            Buttons = null;
        }

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
        public Rectangle GetBaseLayerSourceRectangle(int baseLayerValue)
        {
            if (baseLayerValue < 0)
            {
                return Rectangle.Empty;
            }
            else if (baseLayerValue > 15)
            {
                // Pick a random floor tile (Row 2)
                int index = floorTileRandom.Next(NumberOfFloorTiles);
                return new Rectangle(
                    index * SourceTileSize.X,
                    2 * SourceTileSize.Y,
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
        public Rectangle GetBaseLayerSourceRectangle(Point mapPosition)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((mapPosition.X < 0) || (mapPosition.X >= MapDimensions.X) ||
                (mapPosition.Y < 0) || (mapPosition.Y >= MapDimensions.Y))
            {
                return Rectangle.Empty;
            }

            return GetBaseLayerSourceRectangle(GetBaseLayerValue(mapPosition));
            
        }

        public List<Rectangle> GetWallCornerRectangles()
        {
            List<Rectangle> result = new List<Rectangle>();

            for(int i = 0; i < 4; i++)
            {
                result.Add(new Rectangle(
                    i * SourceTileSize.X,
                    SourceTileSize.Y,
                    SourceTileSize.X,
                    SourceTileSize.Y));
            }
            return result;
        }

        public Vector2 CreateEntityTileCenteredPosition(Rectangle entitySourceRectangle, Vector2 scale, Point tileCoordinates)
        {
            var tilePos = TileCoordinateToPosition(tileCoordinates);
            var spriteCenter = tilePos + new Vector2(entitySourceRectangle.Width * scale.X / 2, entitySourceRectangle.Height * scale.Y / 2);
            var tileCenter = tilePos + new Vector2(TileSize.X / 2, TileSize.Y / 2);
            return tilePos + tileCenter - spriteCenter;
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
        public Point FindClosestFloor(Vector2 position, List<Point> blockedTileCoordinates)
        {
            Point currentTileCoord = PositionToTileCoordinate(position);
            for(int i = 0; i <  MapDimensions.X; i++) 
            {
                for(int x = -i; x <= i; x++)
                {
                    for (int y = -i; y <= i; y++)
                    {
                        Point candidate = currentTileCoord + new Point(x, y);
                        if(ValidTileCoordinates(candidate) && GetCollisionLayerValue(candidate) == 0 && !blockedTileCoordinates.Contains(candidate))
                        {
                            return candidate;
                        }
                    }
                }
            }
            return currentTileCoord;
        }

        public void TranslateCollisionToBaseLayer()
        {
            BaseLayer = new int[CollisionLayer.Length];
            int mapDimensionsX = MapDimensions.X;
            int mapDimensionsY = MapDimensions.Y;
            for (int x = 0; x < mapDimensionsX; x++)
            {
                for (int y = 0; y < mapDimensionsY; y++)
                {
                    bool center, up, down, left, right;
                    if (ValidTileCoordinates(new Point(x, y)))                  {
                        center = CollisionLayer[y * mapDimensionsX + x] != 0;
                    }
                    else
                    {
                        center = true;
                    }
                    if (ValidTileCoordinates(new Point(x, y - 1)))
                    {
                        up = CollisionLayer[(y - 1) * mapDimensionsX + x] != 0;
                    }
                    else
                    {
                        up = true;
                    }
                    if (ValidTileCoordinates(new Point(x, y + 1)))
                    {
                        down = CollisionLayer[(y + 1) * mapDimensionsX + x] != 0;
                    }
                    else
                    {
                        down = true;
                    }
                    if (ValidTileCoordinates(new Point(x - 1, y)))
                    {
                        left = CollisionLayer[y * mapDimensionsX + x - 1] != 0;
                    }
                    else
                    {
                        left = true;
                    }

                    if (ValidTileCoordinates(new Point(x + 1, y)))
                    {
                        right = CollisionLayer[y * mapDimensionsX + x + 1] != 0;
                    }
                    else
                    {
                        right = true;
                    }
                    if (center)
                    {
                        if (!up & !down & !left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 0;
                        }
                        if (up & !down & !left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 1;
                        }
                        if (!up & down & !left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 2;
                        }
                        if (!up & !down & left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 3;
                        }
                        if (!up & !down & !left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 4;
                        }
                        if (up & down & !left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 5;
                        }
                        if (up & !down & left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 6;
                        }
                        if (up & !down & !left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 7;
                        }
                        if (!up & down & left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 8;
                        }
                        if (!up & down & !left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 9;
                        }
                        if (!up & !down & left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 10;
                        }
                        if (up & down & left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 11;
                        }
                        if (up & down & !left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 12;
                        }
                        if (up & !down & left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 13;
                        }
                        if (!up & down & left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 14;
                        }
                        if (up & down & left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 15;
                        }
                    }
                    else
                    {
                        if (!up & !down & !left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 16;
                        }
                        if (up & !down & !left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 17;
                        }
                        if (!up & down & !left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 18;
                        }
                        if (!up & !down & left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 19;
                        }
                        if (!up & !down & !left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 20;
                        }
                        if (up & down & !left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 21;
                        }
                        if (up & !down & left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 22;
                        }
                        if (up & !down & !left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 23;
                        }
                        if (!up & down & left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 24;
                        }
                        if (!up & down & !left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 25;
                        }
                        if (!up & !down & left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 26;
                        }
                        if (up & down & left & !right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 27;
                        }
                        if (up & down & !left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 28;
                        }
                        if (up & !down & left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 29;
                        }
                        if (!up & down & left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 30;
                        }
                        if (up & down & left & right)
                        {
                            BaseLayer[y * mapDimensionsX + x] = 31;
                        }
                    }
                }
            }
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
                
                map.TextureName = input.ReadString();
                map.Texture = input.ContentManager.Load<Texture2D>(
                    Path.Combine(@"Textures\Maps",
                    map.TextureName));
                map.TilesPerRow = map.Texture.Width / map.SourceTileSize.X;
                map.NumberOfFloorTiles = input.ReadInt32();
                map.WorldScale = input.ReadObject<Vector2>();
                map.TorchProbability = input.ReadSingle();
                map.CollisionLayer = input.ReadObject<int[]>();
                map.BaseLayer = input.ReadObject<int[]>();
                map.EntityProperties = input.ReadObject<EntityProperties>();

                map.TorchTexture = input.ContentManager.Load<Texture2D>(@"Textures\Maps\Torches");
                map.TorchSourceRectangles = new List<Rectangle>() {
                new Rectangle(0, 0, map.SourceTileSize.X, 2 * map.SourceTileSize.Y),
                new Rectangle(0, 2 * map.SourceTileSize.Y, map.SourceTileSize.X, 2 * map.SourceTileSize.Y),
                new Rectangle(0, 4 * map.SourceTileSize.Y, 2 * map.SourceTileSize.X, map.SourceTileSize.Y),
                new Rectangle(0, 5 * map.SourceTileSize.Y, 2 * map.SourceTileSize.X, map.SourceTileSize.Y)};

                map.MapBlockDescriptions = input.ReadObject<List<MapBlockDescription>>();
                return map;
            }
        }
    }
    
}

