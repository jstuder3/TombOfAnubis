using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace TombOfAnubis
{
    public class Ghost : Entity
    {
        public Character Character { get; set; }
        public Ghost(Character character)
        {
            Character = character;
            character.Ghost = this;
            Transform characterTransform = character.GetComponent<Transform>();
            Transform transform = new Transform(characterTransform.Position, characterTransform.Scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(characterTransform.Position, 10f * characterTransform.Scale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Sprite sprite;
            if (character.EntityDescription.GhostAnimation != null)
            {
                Animation animation = new Animation(character.EntityDescription.GhostAnimation, Visibility.Game);
                AddComponent(animation);
                animation.SetActiveClip(AnimationClipType.Dead);

                sprite = new Sprite(character.EntityDescription.GhostTexture, animation.DefaultSourceRectangle, 2, Visibility.Game);
            }
            else
            {
                sprite = new Sprite(character.EntityDescription.GhostTexture, 2, Visibility.Both);
            }
            AddComponent(sprite);
            if (character.EntityDescription.GhostAnimation != null)
            {
                Animation animation = new Animation(character.EntityDescription.GhostAnimation, Visibility.Minimap);
                AddComponent(animation);
                animation.SetActiveClip(AnimationClipType.Idle);
            }
            Player player = new Player((int)character.GetComponent<Player>().PlayerID);
            AddComponent(player);

            Input input = new Input();
            AddComponent(input);

            Movement movement = new Movement(Session.GetInstance().Map.EntityProperties.MaxCharacterMovementSpeed);
            AddComponent(movement);

            Initialize();
        }
    }
}
