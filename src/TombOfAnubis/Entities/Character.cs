using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public enum CharacterType
    {
        PlayerOne, PlayerTwo, PlayerThree, PlayerFour
    }
    public class Character : Entity
    {
        public EntityDescription EntityDescription { get; set; }
        public Ghost Ghost { get; set; }
        public Character(EntityDescription entityDescription)
        {
            EntityDescription = entityDescription;
            Vector2 position = Session.GetInstance().Map.CreateEntityTileCenteredPosition(EntityDescription);

            Transform transform = new Transform(position, EntityDescription.Scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(position, 10f * EntityDescription.Scale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Sprite sprite;
            if(EntityDescription.Animation != null)
            {
                Animation animation = new Animation(EntityDescription.Animation, Visibility.Game);
                AddComponent(animation);

                animation.SetActiveClip(AnimationClipType.Idle);

                sprite = new Sprite(EntityDescription.Texture, animation.DefaultSourceRectangle, 2, Visibility.Both);
            }
            else
            {
                sprite = new Sprite(EntityDescription.Texture, 2, Visibility.Both);
            }
            if (EntityDescription.Animation != null)
            {
                Animation animation = new Animation(EntityDescription.Animation, Visibility.Minimap);
                AddComponent(animation);
                animation.SetActiveClip(AnimationClipType.Idle);
            }
            AddComponent(sprite);

            Enum.TryParse(EntityDescription.Type, out CharacterType type);
            Player player = new Player((int)type);
            AddComponent(player);

            Input input = new Input();
            AddComponent(input);

            Movement movement = new Movement(Session.GetInstance().Map.EntityProperties.MaxCharacterMovementSpeed);
            AddComponent(movement);

            Inventory inventory = new Inventory(1, 3, this);
            AddComponent(inventory);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), false);
            AddComponent(collider);

            Initialize();
        }

    }
}
