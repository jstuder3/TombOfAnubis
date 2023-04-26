using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class MapBlockDescription
    {
        [ContentSerializer(Optional = true)]
        public int MinOccurences { get; set; } = 0;

        [ContentSerializer(Optional = true)]
        public int MaxOccurences { get; set; } = int.MaxValue;

        [ContentSerializer(Optional = true)]
        public bool OccursNPlayerOften { get; set; } = false;
        public int Priority { get; set; }
        public string[] BlockNames { get; set; }

        [ContentSerializerIgnore]
        public List<MapBlock> Blocks { get; set; }
        [ContentSerializerIgnore]
        public int Occurences { get; set; }
        [ContentSerializerIgnore]
        public int BasePriority { get; set; }
        public void Update(bool placed)
        {
            if (placed)
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
                else if (Occurences < MaxOccurences)
                {
                    Priority = BasePriority;
                }
            }
        }
        public bool Valid()
        {
            bool valid = Occurences >= MinOccurences && Occurences <= MaxOccurences;
            return valid;
        }

        public void Reset()
        {
            Occurences = 0;
            Priority = BasePriority;
        }

    }
}
