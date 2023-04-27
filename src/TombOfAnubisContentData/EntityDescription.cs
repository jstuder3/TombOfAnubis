using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace TombOfAnubis
{
    public class EntityDescription
    {
        [ContentSerializer(Optional = true)]
        public string ClassName;
        public Point SpawnTileCoordinate;

        [ContentSerializer(Optional = true)]
        public Vector2 Offset = Vector2.Zero;

        [ContentSerializer(Optional = true)]
        public Vector2 Scale = Vector2.One;

        [ContentSerializer(Optional = true)]
        public string SpriteTextureName;

        [ContentSerializer(Optional = true)]
        public string Type;

        [ContentSerializer(Optional = true)]
        public List<EntityDescription> ConnectedTrapPositions = new List<EntityDescription>();

        [ContentSerializerIgnore]
        public Texture2D Texture;

        [ContentSerializer(Optional = true)]
        public List<AnimationClip> Animation;

        [ContentSerializerIgnore]
        public Texture2D GhostTexture;

        [ContentSerializer(Optional = true)]
        public string GhostSpriteTextureName;

        [ContentSerializer(Optional = true)]
        public List<AnimationClip> GhostAnimation;

        public void Load(ContentManager content, string textureDirectory)
        {
            if(SpriteTextureName != null)
            {
                Texture = content.Load<Texture2D>(Path.Combine(textureDirectory, SpriteTextureName));
            }
            int startPosition = 0; 
            if (Animation != null)
            {
                for (int i = 0; i < Animation.Count; i++)
                {
                    Animation[i].SourceRectangle = new Rectangle(0, startPosition, Animation[i].FrameSize.X, Animation[i].FrameSize.Y);
                    startPosition += Animation[i].FrameSize.Y;
                }
            }
            if (GhostSpriteTextureName != null)
            {
                GhostTexture = content.Load<Texture2D>(Path.Combine(textureDirectory, SpriteTextureName));
            }
            startPosition = 0;
            if (GhostAnimation != null)
            {
                for (int i = 0; i < GhostAnimation.Count; i++)
                {
                    GhostAnimation[i].SourceRectangle = new Rectangle(0, startPosition, GhostAnimation[i].FrameSize.X, GhostAnimation[i].FrameSize.Y);
                    startPosition += GhostAnimation[i].FrameSize.Y;
                }
            }

        }
        public EntityDescription Clone()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<EntityDescription>(serialized);
        }
    }
}
