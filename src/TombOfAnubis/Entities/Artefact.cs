using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Artefact : Entity
    {
        public Artefact(int playerID, Vector2 position, Vector2 scale, Texture2D texture, bool collidable)
        {
            Transform transform = new Transform(position, scale * 0.2f, Visibility.Game); //note that scale is much smaller when the artefact is placed on the altar
            AddComponent(transform);

            Player player = new Player(playerID);
            AddComponent(player);

            Sprite sprite = new Sprite(texture, 2, Visibility.Game);
            AddComponent(sprite);

            Discovery discovery = new Discovery();
            AddComponent(discovery);

            if (collidable)
            {
                RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), true);
                AddComponent(collider);
            }


            //particle effects in the colour of the player

            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(20f, 20f);
            pec.RandomizedSpawnPositionRadius = 20f;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 1;
            pec.Scale = Vector2.One * 0.2f;
            pec.InitialAlpha = 1f;
            pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
            pec.ScalingMode = ScalingMode.Constant;
            pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
            pec.EmitterDuration = 0f;
            pec.ParticleDuration = 4f;
            pec.EmissionFrequency = 5f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 20f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 360f;
            pec.Drag = 0.5f;

            switch (playerID)
            {
                case 0:
                    pec.RandomizedTintMin = Color.Red;
                    pec.RandomizedTintMax = Color.Orange;
                    break;
                case 1:
                    pec.RandomizedTintMin = Color.Green;
                    pec.RandomizedTintMax = Color.LimeGreen;
                    break;
                case 2:
                    pec.RandomizedTintMin = Color.Blue;
                    pec.RandomizedTintMax = Color.DarkBlue;
                    break;
                case 3:
                    pec.RandomizedTintMin = Color.Purple;
                    pec.RandomizedTintMax = Color.Violet;
                    break;
            }

            AddComponent(new ParticleEmitter(pec));

            Initialize();
        }

        public Artefact(int playerID, Vector2 position, Vector2 scale, Vector2 minimapScale, Texture2D texture, bool collidable)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(position, minimapScale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Player player = new Player(playerID);
            AddComponent(player);

            Sprite sprite = new Sprite(texture, 2, Visibility.Both);
            AddComponent(sprite);

            Discovery discovery = new Discovery();
            AddComponent(discovery);

            if (collidable)
            {
                RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), true);
                AddComponent(collider);
            }

            //particle effects in the colour of the player

            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(50f, 50f);
            pec.RandomizedSpawnPositionRadius = 100f;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 1;
            pec.Scale = Vector2.One * 0.4f;
            pec.InitialAlpha = 1f;
            pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
            pec.ScalingMode = ScalingMode.Constant;
            pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
            pec.EmitterDuration = 0f;
            pec.ParticleDuration = 4f;
            pec.EmissionFrequency = 15f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 50f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 360f;
            pec.Drag = 0.5f;

            switch (playerID)
            {
                case 0:
                    pec.RandomizedTintMin = Color.Red;
                    pec.RandomizedTintMax = Color.Orange;
                    break;
                case 1:
                    pec.RandomizedTintMin = Color.Green;
                    pec.RandomizedTintMax = Color.LimeGreen;
                    break;
                case 2:
                    pec.RandomizedTintMin = Color.Blue;
                    pec.RandomizedTintMax = Color.DarkBlue;
                    break;
                case 3:
                    pec.RandomizedTintMin = Color.Purple;
                    pec.RandomizedTintMax = Color.Violet;
                    break;
            }

            AddComponent(new ParticleEmitter(pec));

            Initialize();
        }
    }
}
