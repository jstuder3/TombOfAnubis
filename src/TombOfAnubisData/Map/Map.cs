using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Read a Map object from the content pipeline.
        /// </summary>
    //    public class MapReader : ContentTypeReader<Map>
    //    {
    //        protected override Map Read(ContentReader input, Map existingInstance)
    //        {
    //            Map map = existingInstance;
    //            if (map == null)
    //            {
    //                map = new Map();
    //            }

    //            map.AssetName = input.AssetName;

    //            map.Name = input.ReadString();
    //            map.MapDimensions = input.ReadObject<Point>();
    //            map.TileSize = input.ReadObject<Point>();
    //            map.SpawnMapPosition = input.ReadObject<Point>();

    //            map.TextureName = input.ReadString();
    //            map.texture = input.ContentManager.Load<Texture2D>(
    //                System.IO.Path.Combine(@"Textures\Maps\NonCombat",
    //                map.TextureName));
    //            map.tilesPerRow = map.texture.Width / map.TileSize.X;

    //            map.CombatTextureName = input.ReadString();
    //            map.combatTexture = input.ContentManager.Load<Texture2D>(
    //                System.IO.Path.Combine(@"Textures\Maps\Combat",
    //                map.CombatTextureName));

    //            map.MusicCueName = input.ReadString();
    //            map.CombatMusicCueName = input.ReadString();

    //            map.BaseLayer = input.ReadObject<int[]>();
    //            map.FringeLayer = input.ReadObject<int[]>();
    //            map.ObjectLayer = input.ReadObject<int[]>();
    //            map.CollisionLayer = input.ReadObject<int[]>();
    //            map.Portals.AddRange(input.ReadObject<List<Portal>>());

    //            map.PortalEntries.AddRange(
    //                input.ReadObject<List<MapEntry<Portal>>>());
    //            foreach (MapEntry<Portal> portalEntry in map.PortalEntries)
    //            {
    //                portalEntry.Content = map.Portals.Find(delegate (Portal portal)
    //                {
    //                    return (portal.Name == portalEntry.ContentName);
    //                });
    //            }

    //            map.ChestEntries.AddRange(
    //                input.ReadObject<List<MapEntry<Chest>>>());
    //            foreach (MapEntry<Chest> chestEntry in map.chestEntries)
    //            {
    //                chestEntry.Content = input.ContentManager.Load<Chest>(
    //                    System.IO.Path.Combine(@"Maps\Chests",
    //                    chestEntry.ContentName)).Clone() as Chest;
    //            }

    //            // load the fixed combat entries
    //            Random random = new Random();
    //            map.FixedCombatEntries.AddRange(
    //                input.ReadObject<List<MapEntry<FixedCombat>>>());
    //            foreach (MapEntry<FixedCombat> fixedCombatEntry in
    //                map.fixedCombatEntries)
    //            {
    //                fixedCombatEntry.Content =
    //                    input.ContentManager.Load<FixedCombat>(
    //                    System.IO.Path.Combine(@"Maps\FixedCombats",
    //                    fixedCombatEntry.ContentName));
    //                // clone the map sprite in the entry, as there may be many entries
    //                // per FixedCombat
    //                fixedCombatEntry.MapSprite =
    //                    fixedCombatEntry.Content.Entries[0].Content.MapSprite.Clone()
    //                    as AnimatingSprite;
    //                // play the idle animation
    //                fixedCombatEntry.MapSprite.PlayAnimation("Idle",
    //                    fixedCombatEntry.Direction);
    //                // advance in a random amount so the animations aren't synchronized
    //                fixedCombatEntry.MapSprite.UpdateAnimation(
    //                    4f * (float)random.NextDouble());
    //            }

    //            map.RandomCombat = input.ReadObject<RandomCombat>();

    //            map.QuestNpcEntries.AddRange(
    //                input.ReadObject<List<MapEntry<QuestNpc>>>());
    //            foreach (MapEntry<QuestNpc> questNpcEntry in
    //                map.questNpcEntries)
    //            {
    //                questNpcEntry.Content = input.ContentManager.Load<QuestNpc>(
    //                    System.IO.Path.Combine(@"Characters\QuestNpcs",
    //                    questNpcEntry.ContentName));
    //                questNpcEntry.Content.MapPosition = questNpcEntry.MapPosition;
    //                questNpcEntry.Content.Direction = questNpcEntry.Direction;
    //            }

    //            map.PlayerNpcEntries.AddRange(
    //                input.ReadObject<List<MapEntry<Player>>>());
    //            foreach (MapEntry<Player> playerNpcEntry in
    //                map.playerNpcEntries)
    //            {
    //                playerNpcEntry.Content = input.ContentManager.Load<Player>(
    //                System.IO.Path.Combine(@"Characters\Players",
    //                    playerNpcEntry.ContentName)).Clone() as Player;
    //                playerNpcEntry.Content.MapPosition = playerNpcEntry.MapPosition;
    //                playerNpcEntry.Content.Direction = playerNpcEntry.Direction;
    //            }

    //            map.InnEntries.AddRange(
    //                input.ReadObject<List<MapEntry<Inn>>>());
    //            foreach (MapEntry<Inn> innEntry in
    //                map.innEntries)
    //            {
    //                innEntry.Content = input.ContentManager.Load<Inn>(
    //                    System.IO.Path.Combine(@"Maps\Inns",
    //                    innEntry.ContentName));
    //            }
    //            map.StoreEntries.AddRange(
    //                input.ReadObject<List<MapEntry<Store>>>());
    //            foreach (MapEntry<Store> storeEntry in
    //                map.storeEntries)
    //            {
    //                storeEntry.Content = input.ContentManager.Load<Store>(
    //                    System.IO.Path.Combine(@"Maps\Stores",
    //                    storeEntry.ContentName));
    //            }

    //            return map;
    //        }
    //    }
    }
}
