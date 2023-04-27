using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class MapBlock
    {
        #region serializable
        [ContentSerializer(Optional = true)]
        public int MinPlayers { get; set; }
        [ContentSerializer(Optional = true)]
        public int MaxPlayers { get; set; } = 4;

        public Point Dimensions { get; set; }
        public int[] Tiles { get; set; }
        [ContentSerializer(Optional = true)]
        public List<EntityDescription> Entities { get; set; }
        #endregion


        #region non serializable
        [ContentSerializerIgnore]
        public string Name { get; set; }

        [ContentSerializerIgnore]
        public MapBlockDescription Parent { get; set; }

        [ContentSerializerIgnore]
        public static readonly int FloorValue = 0;
        [ContentSerializerIgnore]
        public static readonly int WallValue = 1;
        [ContentSerializerIgnore]
        public static readonly int EmptyValue = 2;
        [ContentSerializerIgnore]
        public static readonly int InvalidValue = 8;
        [ContentSerializerIgnore]
        public static readonly MapBlock Empty = new MapBlock(new Point(1, 1), new int[] { EmptyValue }, 1);
        public static readonly MapBlock Wall = new MapBlock(new Point(1, 1), new int[] { WallValue }, 1);


        #endregion

        public MapBlock() { }
        private MapBlock(Point dimensions, int[] values, int priority) {
            Dimensions = dimensions;
            Tiles = values;
            //Priority = priority;
        }
        public int GetValue(Point coord)
        {
            return Tiles[coord.X * Dimensions.Y + coord.Y];
        }

        public class MapBlockReader : ContentTypeReader<MapBlock>
        {
            protected override MapBlock Read(ContentReader input, MapBlock existingInstance)
            {
                // Map data
                MapBlock mapBlock = existingInstance;
                if (mapBlock == null)
                {
                    mapBlock = new MapBlock();
                }
                mapBlock.MinPlayers = input.ReadInt32();
                mapBlock.MaxPlayers = input.ReadInt32();
                mapBlock.Dimensions = input.ReadObject<Point>();
                mapBlock.Tiles = input.ReadObject<int[]>();
                mapBlock.Entities = input.ReadObject<List<EntityDescription>>();
                return mapBlock;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
