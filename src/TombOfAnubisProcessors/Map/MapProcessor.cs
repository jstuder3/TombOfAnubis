using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis;

namespace TombOfAnubis
{
    [ContentProcessor(DisplayName = "Map Processor")]
    public class MapProcessor : ContentProcessor<Map, Map>
    {
        private int mapDimensionsX;
        private int mapDimensionsY;
        public override Map Process(Map input, ContentProcessorContext context)
        {
            return input;
        }

        private bool Valid(int x, int y)
        {
            return x > 0 && y > 0 && x < mapDimensionsX && y < mapDimensionsY;
        }
    }
}
