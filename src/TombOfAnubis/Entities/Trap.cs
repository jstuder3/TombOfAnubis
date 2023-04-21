using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public enum TrapType
    {
        StaticSpikes
    }

    public class Trap : Entity
    {

        public TrapType Type;
        private List<Button> connectedButtons = new List<Button>();

        public Trap(TrapType type, Vector2 position, Vector2 scale, Texture2D texture, List<AnimationClip> animationClips)
        {
            Type = type;
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Sprite sprite;
            if (animationClips != null)
            {
                Animation animation = new Animation(animationClips, Visibility.Game);
                AddComponent(animation);

                animation.SetActiveClip(AnimationClipType.ObjectActive);

                sprite = new Sprite(texture, animation.DefaultSourceRectangle, 1, Visibility.Game);
            }
            else
            {
                sprite = new Sprite(texture, 2, Visibility.Game);
            }
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size());
            AddComponent(collider);

            Discovery discovery = new Discovery();
            AddComponent(discovery);

            Initialize();
        }

        public bool IsEnabled()
        {
            return !AllConnectedButtonsPressed();
        }

        public bool UpdateVisuals()
        {
            if (IsEnabled())
            {
                GetComponent<Animation>().SetActiveClip(AnimationClipType.ObjectActive);
                return true;
            }
            else
            {
                GetComponent<Animation>().SetActiveClip(AnimationClipType.ObjectInactive);
                return false;
            }
        }

        // check whether all connected buttons are pressed (e.g. to add "disabling" functionality)
        public bool AllConnectedButtonsPressed()
        {
            // can only press all buttons if there even are any connected
            bool allPressed = connectedButtons.Count() > 0; //true;

            foreach (Button button in connectedButtons)
            {
                allPressed = allPressed && button.IsPressed();
            }

            //if the buttons are of "InstantRelease" or "InstantReleaseWithCooldown" type, then they should turn into permanently pressed buttons if all buttons are activated
            if(allPressed)
            {
                foreach(Button button in connectedButtons)
                {
                    if(button.Type == ButtonType.InstantRelease || button.Type == ButtonType.InstantReleaseWithCooldown)
                    {
                        button.Type = ButtonType.NeverRelease;
                    }
                }
            }

            return allPressed;
        }

        public void ConnectButton(Button button)
        {
            connectedButtons.Add(button);
        }

    }
}
