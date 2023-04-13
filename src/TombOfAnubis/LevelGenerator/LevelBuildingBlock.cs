using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class LevelBuildingBlock
    {
        public static readonly Point SmallestBlockSize = new Point(3, 3);

        private Point dimensions;
        // Needs to be dividable by 3
        public Point Dimensions { 
            get { return dimensions; } 
            set
            {
                dimensions = value;
                BlockGridDimensions = new Point(value.X / 3, value.Y / 3);
            }
        }

        public Point BlockGridDimensions { get; set; }

        // Controlls how likely this block is chosen
        public int Priority { get; set; } 
        
        public int[,] Values { get; set; }

        public bool Written { get; set; } = false;

        public int MaxOccurences { get; set; }

        private List<LevelBuildingBlock> clones;
        private int numPlacedCount = 0;

        public string Name { get; set; }

        public LevelBuildingBlock()
        {
            clones = new List<LevelBuildingBlock>();
            MaxOccurences = 100000;
        }
        public void Update()
        {
            if(numPlacedCount < clones.Count())
            {
                numPlacedCount++;
                if (Name.Equals("A") || Name.Equals("B") || Name.Equals("C"))
                {
                    Priority = 1;
                }
            }
            if(clones.Count >= MaxOccurences)
            {
                Priority = 0;
            }else if((Name.Equals("A") || Name.Equals("B") ||  Name.Equals("C")))
            {
                Priority+=2;
            }
        }
        public void Reset()
        {
            numPlacedCount = 0;
            clones = new List<LevelBuildingBlock>();
        }
        public bool RequirementSatisfied()
        {
            if(Name.Equals("A") || Name.Equals("B") ||  Name.Equals("C"))
            {
                Console.WriteLine("Occurences: "+numPlacedCount+", should: "+MaxOccurences);
                return numPlacedCount == MaxOccurences;
            }
            return numPlacedCount <= MaxOccurences;
        }

        public static LevelBuildingBlock OneByOne(int priority)
        {
            LevelBuildingBlock b = new LevelBuildingBlock();
            b.Dimensions = SmallestBlockSize;
            b.Priority = priority;
            b.Name = "1";
            b.Values = new int[,] { { 1, 0, 1 },
                                    { 0, 0, 0 }, 
                                    { 1, 0, 1 } };
            return b;
        }
        public static LevelBuildingBlock TwoByOne(int priority)
        {
            LevelBuildingBlock b = new LevelBuildingBlock();
            b.Dimensions = new Point(6, 3);
            b.Priority = priority;
            b.Name = "2";
            b.Values = new int[,] { { 1, 1, 1 },
                                    { 1, 1, 1 },
                                    { 0, 0, 1 },
                                    { 1, 0, 1 },
                                    { 1, 0, 1 },
                                    { 1, 0, 1 }};
            return b;
        }

        public static LevelBuildingBlock TwoByTwo(int priority)
        {
            LevelBuildingBlock b = new LevelBuildingBlock();
            b.Dimensions = new Point(6, 6);
            b.Priority = priority;
            b.Name = "3";
            b.Values = new int[,] { { 1, 1, 1, 1, 0, 1, 1},
                                    { 1, 1, 1, 1, 0, 1, 1},
                                    { 0, 0, 0, 0, 0, 1, 1},
                                    { 1, 1, 1, 0, 1, 1, 0},
                                    { 1, 1, 1, 0, 1, 1, 1},
                                    { 1, 1, 1, 0, 1, 1, 1}};
            return b;
        }

        public static LevelBuildingBlock Altar()
        {
            LevelBuildingBlock b = new LevelBuildingBlock();
            b.Dimensions = new Point(6, 6);
            b.Priority = 1;
            b.Name = "B";
            b.MaxOccurences = 1;
            b.Values = new int[,] { { 1, 1, 0, 1, 1, 1},
                                    { 1, 0, 0, 0, 0, 1},
                                    { 0, 0, 0, 0, 0, 1},
                                    { 1, 0, 0, 0, 0, 0},
                                    { 1, 0, 0, 0, 0, 1},
                                    { 1, 1, 1, 0, 1, 1}};
            return b;
        }
        public static LevelBuildingBlock Artefakt(int numPlayers)
        {
            LevelBuildingBlock b = new LevelBuildingBlock();
            b.Dimensions = new Point(6, 3);
            b.Priority = 1;
            b.Name = "A";
            b.MaxOccurences = numPlayers;
            b.Values = new int[,] { { 1, 0, 1 },
                                    { 1, 0, 1 },
                                    { 1, 0, 1 },
                                    { 0, 0, 0 },
                                    { 1, 0, 1 },
                                    { 1, 0, 1 }};
            return b;
        }
        public static LevelBuildingBlock SpawnLocation(int numPlayers)
        {
            LevelBuildingBlock b = new LevelBuildingBlock();
            b.Dimensions = new Point(3, 3);
            b.Priority = 1;
            b.Name = "C";
            b.MaxOccurences = numPlayers;
            b.Values = new int[,] { { 1, 0, 1 },
                                    { 0, 0, 0 },
                                    { 1, 0, 1 } };
            return b;
        }
        public LevelBuildingBlock Clone()
        {
            var clone = new LevelBuildingBlock();
            clone.Dimensions = this.Dimensions;
            clone.Priority = this.Priority;
            clone.Name = this.Name;
            clone.Values = this.Values;
            clones.Add(clone);
            return clone;
        }

    }
}
