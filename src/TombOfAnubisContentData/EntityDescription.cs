using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using TombOfAnubisContentData;

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

        [ContentSerializer(Optional = true)]
        public List<AnimationClip> Animation;

        public void Load(ContentManager content, string textureDirectory)
        {
            int startPosition = 0; 
            Texture = content.Load<Texture2D>(Path.Combine(textureDirectory, SpriteTextureName));
            if (Animation != null)
            {
                for (int i = 0; i < Animation.Count; i++)
                {
                    Animation[i].SourceRectangle = new Rectangle(0, startPosition, Animation[i].FrameSize.X, Animation[i].FrameSize.Y);
                    startPosition += Animation[i].FrameSize.Y;
                }
            }        }
    }
}
