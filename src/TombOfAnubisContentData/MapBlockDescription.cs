using Microsoft.Xna.Framework.Content;

namespace TombOfAnubis
{
    public class MapBlockDescription
    {
        public string Name { get; set; }
        public int Priority { get; set; }

        [ContentSerializer(Optional = true)]
        public int MinOccurences { get; set; } = -1;

        [ContentSerializer(Optional = true)]
        public int MaxOccurences { get; set; } = -1;
    }
}
