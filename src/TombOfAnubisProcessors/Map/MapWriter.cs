using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
            int totalTiles = value.MapDimensions.X * value.MapDimensions.Y;
            if (value.BaseLayer.Length != totalTiles)
            {
                throw new InvalidContentException("Base layer was " +
                    value.BaseLayer.Length.ToString() +
                    " tiles, but the dimensions specify " +
                    totalTiles.ToString() + ".");
            }

            output.Write(value.Name);
            output.WriteObject(value.MapDimensions);
            output.WriteObject(value.SourceTileSize);
            output.WriteObject(value.TileScale);
            output.Write(value.TextureName);
            output.WriteObject(value.CollisionLayer);
            output.WriteObject(value.BaseLayer);
            output.WriteObject(value.EntityProperties);
            output.WriteObject(value.Characters);
            output.WriteObject(value.Anubis);
            output.WriteObject(value.Artefacts);
            output.WriteObject(value.Altar);
            output.WriteObject(value.Dispensers);
            output.WriteObject(value.Fist);
        }
    }
}
