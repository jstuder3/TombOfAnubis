using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class Anubis : Entity
    {
        public Anubis(Vector2 position, Vector2 scale, Texture2D texture, List<AnimationClip> animationClips, int maxMovementSpeed, Map map)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Sprite sprite;
            if (animationClips != null)
            {
                Animation animation = new Animation(animationClips, Visibility.Game);
                AddComponent(animation);

                animation.SetActiveClip(AnimationClipType.Flexing);

                sprite = new Sprite(texture, animation.DefaultSourceRectangle, 2, Visibility.Game);
            }
            else
            {
                sprite = new Sprite(texture, 2, Visibility.Game);
            }
            AddComponent(sprite);

            Movement movement = new Movement(maxMovementSpeed);
            AddComponent(movement);

            AI ai = new AI(map);
            AddComponent(ai);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), false);
            AddComponent(collider);
            
            //floating particles
            ParticleEmitterConfiguration pec = new ParticleEmitterConfiguration();
            pec.LocalPosition = new Vector2(25f, 100f);
            pec.RandomizedSpawnPositionRadius = 50f;
            //doesn't work yet
            pec.ParticlesMoveWithEntity = false;
            pec.Texture = ParticleTextureLibrary.BasicParticle;
            pec.SpriteLayer = 1;
            pec.InitialAlpha = 0.5f;
            pec.AlphaMode = AlphaMode.LinearDecreaseToZero;
            pec.RandomizedTintMin = Color.SlateGray;
            pec.RandomizedTintMax = Color.DimGray;
            pec.Scale = Vector2.One * 0.4f;
            pec.ScalingMode = ScalingMode.Constant;
            pec.RelativeScaleVariation = new Vector2(0.8f, 0.8f);
            pec.EmitterDuration = 0f;
            pec.ParticleDuration = 2f;
            pec.EmissionFrequency = 60f;
            pec.EmissionRate = 1f;
            pec.InitialSpeed = 50f;
            pec.SpawnDirection = new Vector2(0f, -1f);
            pec.SpawnConeDegrees = 90f;
            pec.Gravity = new Vector2(0f, 0f);
            //currently behaves a bit unintuitively
            pec.LocalPointForcePosition = Vector2.Zero;
            pec.PointForceStrength = 0f;
            pec.PointForceUsesQuadraticFalloff = false;
            pec.Gravity = new Vector2(0f, 0f);
            pec.Drag = 0.5f;
            pec.Visibility = Visibility.Game;

            AddComponent(new ParticleEmitter(pec));

            //slow anubis down at the start of the game
            AddComponent(new GameplayEffect(EffectType.MultiplicativeSpeedModification, 5f, 0.001f, Visibility.Both));
            AddComponent(new GameplayEffect(EffectType.MultiplicativeSpeedModification, 10f, 0.5f, Visibility.Both));

        }
    }
}
