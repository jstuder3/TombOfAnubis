using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TombOfAnubis
{
    [ContentTypeWriter]
    public class MapBlockWriter : ContentTypeWriter<MapBlock>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "TombOfAnubis.MapBlock+MapBlockReader, TombOfAnubisContentData";
        }

        protected override void Write(ContentWriter output, MapBlock value)
        {
            output.WriteObject(value.Dimensions);
            output.WriteObject(value.Tiles);
            output.WriteObject(value.Entities);
        }
    }
}
