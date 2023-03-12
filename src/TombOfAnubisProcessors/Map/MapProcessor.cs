using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis;

namespace TombOfAnubisProcessors
{
    [ContentProcessor(DisplayName = "Map Processor")]
    public class MapProcessor : ContentProcessor<Map, Map>
    {
        public override Map Process(Map input, ContentProcessorContext context)
        {
            System.Diagnostics.Debug.WriteLine("Reader:  " + input.Name);
            input.Name = "processed map";
            return input;
        }
    }
}
