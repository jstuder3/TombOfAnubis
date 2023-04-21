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
    public enum ButtonType
    {
        InstantRelease,
        NeverRelease,
        TimedRelease,
        InstantReleaseWithCooldown,
        TimedReleaseWithCooldown
    }
    public class Button : Entity
    {
        public ButtonType Type;

        public Button(ButtonType type, Vector2 position, Vector2 scale, Texture2D texture, List<AnimationClip> animationClips, List<Vector2> positionsOfTrapsToConnect)
        {
            Type = type;

            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Sprite sprite;
            if (animationClips != null)
            {
                Animation animation = new Animation(animationClips, Visibility.Game);
                AddComponent(animation);

                animation.SetActiveClip(AnimationClipType.NotPressed);

                sprite = new Sprite(texture, animation.DefaultSourceRectangle, 1, Visibility.Game);
            }
            else
            {
                sprite = new Sprite(texture, 1, Visibility.Game);
            }
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size());
            AddComponent(collider);

            Discovery discovery = new Discovery();
            AddComponent(discovery);

            Session singleton = Session.GetInstance();

            float tolerance = 10f;

            //connect to all traps for now (note that buttons should always be initialized _after_ all traps are spawned)
            /*List<Trap> connectedTraps = new List<Trap>();
            foreach (Trap trap in singleton.Scene.GetChildrenOfType<Trap>())
            {
                connectedTraps.Add(trap);
                trap.ConnectButton(this);
                
            }*/

            // iterate over list of trap positions, add all traps that are close to those positions
            List<Trap> connectedTraps = new List<Trap>();
            foreach (Trap trap in singleton.World.GetChildrenOfType<Trap>())
            {
                foreach(Vector2 targetTrapPosition in positionsOfTrapsToConnect)
                {
                    float distance = (trap.GetComponent<RectangleCollider>().GetCenter() - targetTrapPosition).Length();
                    if (distance <= tolerance)
                    {
                        if (!connectedTraps.Contains<Trap>(trap))
                        {
                            connectedTraps.Add(trap);
                            trap.ConnectButton(this);
                        }
                    }
                }
            }

            ButtonController buttonController = new ButtonController(connectedTraps);
            AddComponent(buttonController);

            Initialize();
        }

        public bool IsPressed()
        {
            return GetComponent<ButtonController>().IsPressed();
        }
        


    }
}
