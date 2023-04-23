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
    [ContentProcessor(DisplayName = "MapBlock Processor")]
    public class MapBlockProcessor : ContentProcessor<MapBlock, MapBlock>
    {
        public override MapBlock Process(MapBlock input, ContentProcessorContext context)
        {
            input.BasePriority = input.Priority;
            if(input.MinOccurences == -1){
                input.MinOccurences = 0;
            }
            if (input.MaxOccurences == -1)
            {
                input.MaxOccurences = int.MaxValue;
            }
            return input;
        }
    }
}
