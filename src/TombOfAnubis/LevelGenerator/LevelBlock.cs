using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class LevelBlock
    {
        public static readonly int FloorValue = 0;
        public static readonly int WallValue = 1;
        public static readonly int EmptyValue = 2;
        public static readonly int InvalidValue = 8;

        public static readonly LevelBlock Empty = new LevelBlock(new int[,] { { EmptyValue } }, 1, "Empty");

        public int MaxOccurences { get; set; }
        public int MinOccurences { get; set; }
        public int Occurences { get; set; }
        public Point Dimensions { get; set; }
        public int Priority { get; set; }
        public int[,] Values { get; set; }

        public string Name { get; set; }
        private int basePriority;


        public LevelBlock(int[,] values, int priority, int minOccurences, int maxOccurences) 
        {
            Occurences = 0;
            Priority = priority;
            basePriority = priority;
            MinOccurences = minOccurences;
            MaxOccurences = maxOccurences;
            this.Values = values;
            Dimensions = new Point(values.GetLength(0), values.GetLength(1));
        }
        public LevelBlock(int[,] values, int priority, string name) : this(values, priority, 0, int.MaxValue) { Name = name; }


        public int GetValue(Point coord)
        {
            return Values[coord.X, coord.Y];
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
                    Priority = basePriority;
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
                    Priority = basePriority;
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
            Priority = basePriority;
        }
    }
}
