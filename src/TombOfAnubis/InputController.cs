using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public enum PlayerActions
    {
        WalkLeft,
        WalkRight,
        WalkUp,
        WalkDown,
        UseObject
    }
    public static class InputController
    {

        public static Keys[] upKeys = new Keys[] { Keys.W, Keys.T, Keys.I, Keys.Up};
        public static Keys[] leftKeys = new Keys[] {Keys.A, Keys.F, Keys.J, Keys.Left };
        public static Keys[] downKeys = new Keys[] { Keys.S, Keys.G, Keys.K, Keys.Down };
        public static Keys[] rightKeys = new Keys[] { Keys.D, Keys.H, Keys.L, Keys.Right };
        public static Keys[] useKeys = new Keys[] { Keys.E, Keys.E, Keys.E, Keys.E };

        public static Buttons[] upButtons = new Buttons[] { Buttons.DPadUp, Buttons.DPadUp, Buttons.DPadUp, Buttons.DPadUp };
        public static Buttons[] leftButtons = new Buttons[] {Buttons.DPadLeft, Buttons.DPadLeft, Buttons.DPadLeft, Buttons.DPadLeft };
        public static Buttons[] downButtons = new Buttons[] {Buttons.DPadDown, Buttons.DPadDown, Buttons.DPadDown, Buttons.DPadDown };
        public static Buttons[] rightButtons = new Buttons[] {Buttons.DPadRight, Buttons.DPadRight, Buttons.DPadRight, Buttons.DPadRight };
        public static Buttons[] useButtons = new Buttons[] {Buttons.A, Buttons.A, Buttons.A, Buttons.A };


        //convert key presses by the current player into actions (walking left, right, up down, use, ...)
        public static PlayerActions[] GetActionsOfCurrentPlayer(int player)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            GamePadState gamepadState = GamePadState.Default;

            if (player == 0) gamepadState = GamePad.GetState(PlayerIndex.One);
            if (player == 1) gamepadState = GamePad.GetState(PlayerIndex.Two);
            if (player == 2) gamepadState = GamePad.GetState(PlayerIndex.Three); 
            if (player == 3) gamepadState = GamePad.GetState(PlayerIndex.Four);

            Buttons[] pressedButtons = GetPressedButtons(gamepadState);

            List<PlayerActions> actions = new List<PlayerActions>();


            foreach(Keys key in pressedKeys)
            {
                if (key == upKeys[player])
                {
                    actions.Add(PlayerActions.WalkUp);
                }

                if (key == leftKeys[player])
                {
                    actions.Add(PlayerActions.WalkLeft);
                }

                if (key == downKeys[player])
                {
                    actions.Add(PlayerActions.WalkDown);
                }

                if (key == rightKeys[player])
                {
                    actions.Add(PlayerActions.WalkRight);
                }

                if (key == useKeys[player])
                {
                    actions.Add(PlayerActions.UseObject);
                }
            }

            foreach(Buttons button in pressedButtons)
            {
                if (button == upButtons[player])
                {
                    actions.Add(PlayerActions.WalkUp);
                }

                if (button == leftButtons[player])
                {
                    actions.Add(PlayerActions.WalkLeft);
                }

                if (button == downButtons[player])
                {
                    actions.Add(PlayerActions.WalkDown);
                }

                if (button == rightButtons[player])
                {
                    actions.Add(PlayerActions.WalkRight);
                }

                if (button == useButtons[player])
                {
                    actions.Add(PlayerActions.UseObject);
                }
            }

            //remove up/down and left/right conflicts, as well as duplicate keys
            actions = HandleOpposingActions(actions);

            return actions.ToArray();


        }

        //there is no equivalent to GetPressedKeys() of KeyboardState for GamePadState, so we have to implement it ourselves
        //(adapted from https://community.monogame.net/t/get-all-currently-pressed-gamepad-buttons-similar-to-keyboardstates-getpressedkeys/17966) 
        public static Buttons[] GetPressedButtons(GamePadState gamepadState)
        {
            List<Buttons> pressedButtons = new List<Buttons>();

            foreach (Buttons btn in Enum.GetValues(typeof(Buttons)))
            {
                if (gamepadState.IsButtonDown(btn))
                {
                    pressedButtons.Add(btn);
                }
            }

            return pressedButtons.ToArray();
        }

        public static List<PlayerActions> HandleOpposingActions(List<PlayerActions> actions)
        {
            List<PlayerActions> resolvedActions = new List<PlayerActions> ();

            //if up and down are pressed at the same time, we don't add them to the list of player inputs
            if(actions.Contains(PlayerActions.WalkUp) && !actions.Contains(PlayerActions.WalkDown))
            {
                resolvedActions.Add(PlayerActions.WalkUp);
            }
            if (!actions.Contains(PlayerActions.WalkUp) && actions.Contains(PlayerActions.WalkDown))
            {
                resolvedActions.Add(PlayerActions.WalkDown);
            }

            //if left and right are pressed at the same time, we don't add them to the list of player inputs
            if (actions.Contains(PlayerActions.WalkLeft) && !actions.Contains(PlayerActions.WalkRight))
            {
                resolvedActions.Add(PlayerActions.WalkLeft);
            }
            if (!actions.Contains(PlayerActions.WalkLeft) && actions.Contains(PlayerActions.WalkRight))
            {
                resolvedActions.Add(PlayerActions.WalkRight);
            }

            //for all remaining actions, just append them to the list if they're not already contained
            foreach(PlayerActions action in actions)
            {
                if(action != PlayerActions.WalkUp && action != PlayerActions.WalkDown && action != PlayerActions.WalkLeft && action != PlayerActions.WalkRight && !resolvedActions.Contains(action))
                {
                    resolvedActions.Add(action);
                }
            }

            return resolvedActions;

        }
    }
}
