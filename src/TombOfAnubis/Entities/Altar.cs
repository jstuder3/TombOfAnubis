using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Altar : Entity
    {
        public Altar(Vector2 position, Vector2 scale, Texture2D texture, int numPlayers)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(position, 2f * scale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Sprite sprite = new Sprite(texture, 1, Visibility.Both);
            AddComponent(sprite);

            Inventory inventory = new Inventory(numPlayers, 0, this);
            AddComponent(inventory);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), true);
            AddComponent(collider);

            Discovery discovery = new Discovery();
            AddComponent(discovery);
            
            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(200f, 30f);
            pec.RandomizedSpawnPositionRadius = 20f;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 1;
            pec.RandomizedTintMin = Color.Red;
            pec.RandomizedTintMax = Color.Orange;
            pec.Scale = Vector2.One * 0.2f;
            pec.InitialAlpha = 0.8f;
            pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
            pec.ScalingMode = ScalingMode.Constant;
            pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
            pec.EmitterDuration = 0f;
            pec.ParticleDuration = 1f;
            pec.EmissionFrequency = 30f;
            pec.EmissionRate = 1f;
            pec.SpriteLayer = 3;
            pec.InitialSpeed = 150f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 90f;
            pec.Drag = 0.8f;
            pec.Visibility = Visibility.Game;

            AddComponent(new ParticleEmitter(pec));
            

            Initialize();
        }
    }
}
