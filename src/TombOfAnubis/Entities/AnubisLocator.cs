using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class AnubisLocator : Entity
    {
        public AnubisLocator(Vector2 position, Vector2 scale)
        {
            Transform transform = new Transform(position, scale, Visibility.Both);
            AddComponent(transform);

            /*Transform minimapTransform = new Transform(position, 1f *scale, Visibility.Minimap);
            AddComponent(minimapTransform);*/

            Sprite sprite = new Sprite(ParticleTextureLibrary.BasicParticle, 3, Visibility.Minimap);
            sprite.Tint = Color.Red;
            AddComponent(sprite);

            /*ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(0f, 0f); //Session.GetInstance().World.GetChildrenOfType<Anubis>()[0].CenterPosition();
            pec.RandomizedSpawnPositionRadius = 0f;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 3;
            pec.RandomizedTintMin = Color.Red;
            pec.RandomizedTintMax = Color.DarkRed;
            pec.Scale = Vector2.One * 10f;
            pec.ScalingMode = ScalingMode.LinearDecreaseToZero;
            pec.RelativeScaleVariation = new Vector2(0.2f, 0.2f);
            pec.EmitterDuration = 4f;
            pec.ParticleDuration = 1f;
            pec.EmissionFrequency = 5f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 0f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 360f;
            pec.Drag = 0.5f;
            pec.Visibility = Visibility.Minimap;

            AddComponent(new ParticleEmitter(pec));*/

            //Entity.AddComponent(new Sprite(ItemTextureLibrary.HidingCloak, 3, Visibility.Both));


        }
    }
}
