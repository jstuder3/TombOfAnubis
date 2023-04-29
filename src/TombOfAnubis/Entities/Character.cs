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
            Enum.TryParse(EntityDescription.Type, out CharacterType type);
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
                switch (type)
                {
                    case CharacterType.PlayerOne:
                        animation.SetActiveClip(AnimationClipType.WalkingLeft); break;
                    case CharacterType.PlayerTwo:
                        animation.SetActiveClip(AnimationClipType.WalkingUp); break;
                    case CharacterType.PlayerThree:
                        animation.SetActiveClip(AnimationClipType.WalkingDown); break;
                    case CharacterType.PlayerFour:
                        animation.SetActiveClip(AnimationClipType.WalkingRight); break;

                }

                sprite = new Sprite(EntityDescription.Texture, animation.DefaultSourceRectangle, 2, Visibility.Game);
                AddComponent(sprite);
            }
            Sprite minimapSprite = new Sprite(Session.GetInstance().MinimapTexture, Session.GetInstance().MinimapCharacterSourceRectangles[(int)type], 2, Visibility.Minimap);
            AddComponent(minimapSprite);

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
