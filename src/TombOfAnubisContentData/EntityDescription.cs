using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace TombOfAnubis
{
    public class EntityDescription
    {
        public Point SpawnTileCoordinate;

        [ContentSerializer(Optional = true)]
        public Vector2 Offset = Vector2.Zero;

        [ContentSerializer(Optional = true)]
        public Vector2 Scale = Vector2.One;

        [ContentSerializer(Optional = true)]
        public string SpriteTextureName;

        [ContentSerializer(Optional = true)]
        public string Type;

        [ContentSerializerIgnore]
        public Texture2D Texture;
    }
}
