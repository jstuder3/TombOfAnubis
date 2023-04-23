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
        public Point Dimensions { get; set; }
        public int[] Tiles { get; set; }
        [ContentSerializer(Optional = true)]
        public List<EntityDescription> Entities { get; set; }
        #endregion


        #region non serializable
        [ContentSerializerIgnore]
        public int MinOccurences { get; set; } = -1;
        [ContentSerializerIgnore]
        public int MaxOccurences { get; set; } = -1;
        [ContentSerializerIgnore]
        public int Priority { get; set; }
        [ContentSerializerIgnore]
        public int Occurences { get; set; }
        [ContentSerializerIgnore]
        public int BasePriority {get; set; }

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

        #endregion

        public MapBlock() { }
        private MapBlock(Point dimensions, int[] values, int priority) {
            Dimensions = dimensions;
            Tiles = values;
            Priority = priority;
        }
        public int GetValue(Point coord)
        {
            return Tiles[coord.X * Dimensions.Y + coord.Y];
        }

        public void Update(bool placed)
        {
            if(placed)
            {
                Occurences++;
                if (Occurences < MinOccurences)
                {
                    Priority = 1;
                }
                else if (Occurences < MaxOccurences)
                {
                    Priority = BasePriority;
                }
                else
                {
                    Priority = 0;
                }
            }
            else
            {
                if (Occurences < MinOccurences)
                {
                    Priority++;
                }
                else if(Occurences < MaxOccurences)
                {
                    Priority = BasePriority;
                }
            }
        }
        public bool Valid()
        {
            bool valid = Occurences >= MinOccurences && Occurences <= MaxOccurences;
            //if (!valid)
            //{
            //    Console.WriteLine("Occurences: " + Occurences + ", Min: " + MinOccurences + ", Max: " + MaxOccurences);
            //}
            return valid;
        }

        public void Reset()
        {
            Occurences = 0;
            Priority = BasePriority;
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
                mapBlock.Dimensions = input.ReadObject<Point>();
                mapBlock.Tiles = input.ReadObject<int[]>();
                mapBlock.Entities = input.ReadObject<List<EntityDescription>>();
                return mapBlock;
            }
        }
    }
}
