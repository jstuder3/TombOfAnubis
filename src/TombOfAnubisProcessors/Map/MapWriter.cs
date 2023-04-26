using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TombOfAnubis
{
    [ContentTypeWriter]
    public class MapWriter : ContentTypeWriter<Map>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "TombOfAnubis.Map+MapReader, TombOfAnubisContentData";
        }

        protected override void Write(ContentWriter output, Map value)
        {

            // validate the map first
            if (value.MapDimensions.X <= 0 ||
                value.MapDimensions.Y <= 0)
            {
                throw new InvalidContentException("Invalid map dimensions.");
            }


            output.Write(value.Name);
            output.WriteObject(value.MapDimensions);
            output.WriteObject(value.SourceTileSize);
            output.WriteObject(value.TileScale);
            output.Write(value.TextureName);
            output.Write(value.NumberOfFloorTiles);
            output.WriteObject(value.WorldScale);
            output.Write(value.TorchProbability);
            output.WriteObject(value.CollisionLayer);
            output.WriteObject(value.BaseLayer);
            output.WriteObject(value.EntityProperties);
            output.WriteObject(value.MapBlockDescriptions);
            //output.WriteObject(value.Characters);
            //output.WriteObject(value.Anubis);
            //output.WriteObject(value.Artefacts);
            //output.WriteObject(value.Altar);
            //output.WriteObject(value.Dispensers);
            //output.WriteObject(value.Traps);
            //output.WriteObject(value.Buttons);
            //output.WriteObject(value.Fist);
        }
    }
}
