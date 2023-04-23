using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class Character : Entity
    {
        public Character(int playerID, Vector2 position, Vector2 scale, Texture2D texture, int maxMovementSpeed, List<AnimationClip> animationClips, List<AnimationClip> minimapAnimationClips)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(position, 10f * scale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Sprite sprite;
            if(animationClips != null)
            {
                Animation animation = new Animation(animationClips, Visibility.Game);
                AddComponent(animation);

                animation.SetActiveClip(AnimationClipType.Idle);

                sprite = new Sprite(texture, animation.DefaultSourceRectangle, 2, Visibility.Both);
            }
            else
            {
                sprite = new Sprite(texture, 2, Visibility.Both);
            }
            if(minimapAnimationClips != null)
            {
                Animation animation = new Animation(minimapAnimationClips, Visibility.Minimap);
                AddComponent(animation);
                animation.SetActiveClip(AnimationClipType.Idle);
            }
            AddComponent(sprite);

            Player player = new Player(playerID);
            AddComponent(player);

            Input input = new Input();
            AddComponent(input);

            Movement movement = new Movement(maxMovementSpeed);
            AddComponent(movement);

            Inventory inventory = new Inventory(1, 3, this);
            AddComponent(inventory);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), false);
            AddComponent(collider);

            Initialize();
        }

    }
}
