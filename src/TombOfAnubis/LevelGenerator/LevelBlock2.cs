using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace TombOfAnubis
{
    public class LevelBlock2
    {
        //public static readonly Point BlockSize = new Point(1, 1);
        public LevelBlock LevelPiece { get; set; }


        //public Point BlockGridDimensions { get; set; }

        // Controlls how likely this block is chosen
        //public int Priority { get; set; } 
        
        public int Value { get; set; }

        //public bool Written { get; set; } = false;

        //public int MaxOccurences { get; set; }

        //private List<LevelBlock> clones;
        //private int numPlacedCount = 0;

        public string Name { get; set; }

        public static readonly LevelBlock2 Empty = new LevelBlock2(2, "f");

        public LevelBlock2 (int value, string name)
        {
            Value = value;
            Name = name;
        }

        //public LevelBlock()
        //{
        //    //clones = new List<LevelBlock>();
        //    //MaxOccurences = 100000;
        //}
        //public void Update()
        //{
        //    if(numPlacedCount < clones.Count())
        //    {
        //        numPlacedCount++;
        //        if (Name.Equals("A") || Name.Equals("B") || Name.Equals("C"))
        //        {
        //            Priority = 1;
        //        }
        //    }
        //    if(clones.Count >= MaxOccurences)
        //    {
        //        Priority = 0;
        //    }else if((Name.Equals("A") || Name.Equals("B") ||  Name.Equals("C")))
        //    {
        //        Priority+=2;
        //    }
        //}
        //public void Reset()
        //{
        //    numPlacedCount = 0;
        //    clones = new List<LevelBlock>();
        //}
        //public bool RequirementSatisfied()
        //{
        //    if(Name.Equals("A") || Name.Equals("B") ||  Name.Equals("C"))
        //    {
        //        Console.WriteLine("Occurences: "+numPlacedCount+", should: "+MaxOccurences);
        //        return numPlacedCount == MaxOccurences;
        //    }
        //    return numPlacedCount <= MaxOccurences;
        //}

        //public static LevelBlock OneByOne(int priority)
        //{
        //    LevelBlock b = new LevelBlock();
        //    b.Dimensions = BlockSize;
        //    b.Priority = priority;
        //    b.Name = "1";
        //    b.Values = new int[,] { { 1, 0, 1 },
        //                            { 0, 0, 0 }, 
        //                            { 1, 0, 1 } };
        //    return b;
        //}
        //public static LevelBlock TwoByOne(int priority)
        //{
        //    LevelBlock b = new LevelBlock();
        //    b.Dimensions = new Point(6, 3);
        //    b.Priority = priority;
        //    b.Name = "2";
        //    b.Values = new int[,] { { 1, 1, 1 },
        //                            { 1, 1, 1 },
        //                            { 0, 0, 1 },
        //                            { 1, 0, 1 },
        //                            { 1, 0, 1 },
        //                            { 1, 0, 1 }};
        //    return b;
        //}

        //public static LevelBlock TwoByTwo(int priority)
        //{
        //    LevelBlock b = new LevelBlock();
        //    b.Dimensions = new Point(6, 6);
        //    b.Priority = priority;
        //    b.Name = "3";
        //    b.Values = new int[,] { { 1, 1, 1, 1, 0, 1, 1},
        //                            { 1, 1, 1, 1, 0, 1, 1},
        //                            { 0, 0, 0, 0, 0, 1, 1},
        //                            { 1, 1, 1, 0, 1, 1, 0},
        //                            { 1, 1, 1, 0, 1, 1, 1},
        //                            { 1, 1, 1, 0, 1, 1, 1}};
        //    return b;
        //}

        //public static LevelBlock Altar()
        //{
        //    LevelBlock b = new LevelBlock();
        //    b.Dimensions = new Point(6, 6);
        //    b.Priority = 1;
        //    b.Name = "B";
        //    b.MaxOccurences = 1;
        //    b.Values = new int[,] { { 1, 1, 0, 1, 1, 1},
        //                            { 1, 0, 0, 0, 0, 1},
        //                            { 0, 0, 0, 0, 0, 1},
        //                            { 1, 0, 0, 0, 0, 0},
        //                            { 1, 0, 0, 0, 0, 1},
        //                            { 1, 1, 1, 0, 1, 1}};
        //    return b;
        //}
        //public static LevelBlock Artefakt(int numPlayers)
        //{
        //    LevelBlock b = new LevelBlock();
        //    b.Dimensions = new Point(6, 3);
        //    b.Priority = 1;
        //    b.Name = "A";
        //    b.MaxOccurences = numPlayers;
        //    b.Values = new int[,] { { 1, 0, 1 },
        //                            { 1, 0, 1 },
        //                            { 1, 0, 1 },
        //                            { 0, 0, 0 },
        //                            { 1, 0, 1 },
        //                            { 1, 0, 1 }};
        //    return b;
        //}
        //public static LevelBlock SpawnLocation(int numPlayers)
        //{
        //    LevelBlock b = new LevelBlock();
        //    b.Dimensions = new Point(3, 3);
        //    b.Priority = 1;
        //    b.Name = "C";
        //    b.MaxOccurences = numPlayers;
        //    b.Values = new int[,] { { 1, 0, 1 },
        //                            { 0, 0, 0 },
        //                            { 1, 0, 1 } };
        //    return b;
        //}
        //public LevelBlock Clone()
        //{
        //    var clone = new LevelBlock();
        //    clone.Dimensions = this.Dimensions;
        //    clone.Priority = this.Priority;
        //    clone.Name = this.Name;
        //    clone.Values = this.Values;
        //    clones.Add(clone);
        //    return clone;
        //}

    }
}
