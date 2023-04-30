using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class ButtonController : Component
    {
        public List<Trap> ConnectedTraps; //not yet in use

        private bool somebodyIsStandingOnButton = false;
        private bool isPressed = false;

        // the following variable is used for TimedRelease buttons (i.e. the button stays pressed for a short time after pressing it and then reverts to being "not pressed")
        public float ReleaseDelay = 5f;
        public float ReleaseEndTime = 0f;

        //used for "cooldown" on buttons, which are button type that have a cooldown period when they are released, during which they cannot be pressed again
        public float Cooldown = 5f;
        public float CooldownEnd = 0f;

        //could be used for vfx in the future, where the button starts blinking / making sounds when it's close to being released
        public float PanicTimer = 3f;

        public ButtonController(List<Trap> connectedTraps) {
            ConnectedTraps = connectedTraps;
            ButtonControllerSystem.Register(this);
        }

        public void Update(GameTime gameTime)
        {
            UpdateVisuals();
            Button button = (Button)Entity;

            //ensure button is not on cooldown
            if (ButtonIsOnCooldown(gameTime))
            {
                isPressed = false;
                UpdateVisuals();
                return;
            }

            //loop over all overlapping colliders and figure out whether a player is standing on the button (could alternatively loop over all entities in the scene and find the distance to all players)
            foreach (Collider collider in button.GetComponent<RectangleCollider>().OverlappingColliders)
            {
                if (collider.Entity.GetType().Name == nameof(Character))
                {
                    somebodyIsStandingOnButton = true;
                    isPressed = true;
                    UpdateVisuals();
                    return;
                }
            }

            //"else", i.e. if nobody is pressing the button in the current frame and somebody previously pressed the button (i.e. they moved off of the button)
            if (somebodyIsStandingOnButton)
            {
                if (button.Type == ButtonType.InstantRelease) //reset variable, button is not pressed anymore
                {
                    somebodyIsStandingOnButton = false;
                    isPressed = false;
                }
                if (button.Type == ButtonType.InstantReleaseWithCooldown) //reset variable, button is not pressed anymore and is on cooldown
                {
                    somebodyIsStandingOnButton = false;
                    isPressed = false;
                    CooldownEnd = (float)gameTime.TotalGameTime.TotalSeconds + Cooldown;
                }
                if (button.Type == ButtonType.NeverRelease) //don't reset variable, button remains pressed forever
                {
                    isPressed = true;
                }
                if (button.Type == ButtonType.TimedRelease || button.Type == ButtonType.TimedReleaseWithCooldown) //reset variable, button remains pressed, but only until the time runs out
                {
                    somebodyIsStandingOnButton = false;
                    isPressed = true;
                    ReleaseEndTime = (float)gameTime.TotalGameTime.TotalSeconds + ReleaseDelay;
                }

                UpdateVisuals();

            }
            else
            {
                //if nobody is currently pressing the button and this is a TimedRelease button, wait for release time to release
                if ((button.Type == ButtonType.TimedRelease || button.Type == ButtonType.TimedReleaseWithCooldown) && (float)gameTime.TotalGameTime.TotalSeconds > ReleaseEndTime)
                {
                    if (button.Type == ButtonType.TimedReleaseWithCooldown) // in case of a cooldown-button, we also put the button on cooldown
                    {
                        CooldownEnd = (float)gameTime.TotalGameTime.TotalSeconds + Cooldown;
                    }
                    isPressed = false;
                }
                else if ((button.Type == ButtonType.TimedRelease || button.Type == ButtonType.TimedReleaseWithCooldown))
                {
                    isPressed = true;
                }

                UpdateVisuals();
            }
        }

        public bool IsPressed()
        {
            return isPressed;
        }

        public void UpdateVisuals()
        {
            if (isPressed)
            {
                Entity.GetComponent<Animation>().SetActiveClip(AnimationClipType.Pressed);
            }
            else
            {
                Entity.GetComponent<Animation>().SetActiveClip(AnimationClipType.NotPressed);
            }

            foreach (Trap connectedTrap in ConnectedTraps)
            {
                connectedTrap.UpdateVisuals();
            }
        }

        public bool ButtonIsOnCooldown(GameTime gameTime)
        {
            Button button = (Button)Entity;
            if (button.Type == ButtonType.InstantReleaseWithCooldown || button.Type == ButtonType.TimedReleaseWithCooldown)
            {
                return (float)gameTime.TotalGameTime.TotalSeconds < CooldownEnd;
            }
            else
            {
                return false;
            }
        }

        //could be used for vfx in the future, where the button starts blinking / making sounds when it's close to being released
        public bool IsCloseToBeingReleased(GameTime gameTime)
        {
            Button button = (Button)Entity;
            if (button.Type == ButtonType.TimedRelease && (float)gameTime.TotalGameTime.TotalSeconds > (ReleaseEndTime - PanicTimer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
