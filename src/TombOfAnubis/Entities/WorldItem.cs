using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class WorldItem : Entity
    {
        public ItemType ItemType { get; set; }
        public WorldItem(Vector2 position, Vector2 scale, ItemType itemType)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Sprite sprite = new Sprite(ItemTextureLibrary.GetTexture(itemType), 0, Visibility.Game);
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), false);
            AddComponent(collider);

            ItemType = itemType;

            Initialize();

            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(20f, 20f);
            pec.RandomizedSpawnPositionRadius = 40f;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 4;
            pec.RandomizedTintMin = Color.Yellow;
            pec.RandomizedTintMax = Color.LightYellow;
            pec.Scale = Vector2.One * 0.2f;
            pec.ScalingMode = ScalingMode.LinearDecreaseToZero;
            pec.RelativeScaleVariation = new Vector2(0.9f, 0.9f);
            pec.EmitterDuration = 0f;
            pec.ParticleDuration = 2f;
            pec.EmissionFrequency = 5f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 10f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 360f;
            pec.Drag = 0.5f;

            AddComponent(new ParticleEmitter(pec));

        }
    new public void Delete()
    {
        base.Delete();
    }
    }


}
