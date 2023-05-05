using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public  class HidingCloak : Entity
    {

        public HidingCloak(Vector2 position, Vector2 scale, Texture2D texture)
        {
            Transform transform = new Transform(position, scale, Visibility.Both);
            AddComponent(transform);

            //Sprite sprite = new Sprite(texture, 3, Visibility.Game);
            //sprite.Alpha = 0.5f;
            //AddComponent(sprite);

            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(20f, 20f);
            pec.RandomizedSpawnPositionRadius = 50f;
            //doesn't work yet
            pec.ParticlesMoveWithEntity = false;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 1;
            pec.InitialAlpha = 0.8f;
            pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
            pec.RandomizedTintMin = new Color(194, 161, 88);
            pec.RandomizedTintMax = new Color(197, 167, 100);
            pec.Scale = Vector2.One * 0.5f;
            pec.ScalingMode = ScalingMode.Constant;
            pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
            pec.EmitterDuration = 4.5f;
            pec.ParticleDuration = 0.5f;
            pec.EmissionFrequency = 60f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 150f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 360f;
            pec.Gravity = new Vector2(0f, 0f);
            pec.SpriteLayer = 2;
            //currently behaves a bit unintuitively
            pec.LocalPointForcePosition = Vector2.Zero;
            pec.PointForceStrength = 0f;
            pec.PointForceUsesQuadraticFalloff = false;
            pec.Gravity = new Vector2(0f, 0f);
            pec.Drag = 0.5f;
            pec.Visibility = Visibility.Game;

            AddComponent(new ParticleEmitter(pec));

            //Entity.AddComponent(new Sprite(ItemTextureLibrary.HidingCloak, 3, Visibility.Both));


        }

    }
}
