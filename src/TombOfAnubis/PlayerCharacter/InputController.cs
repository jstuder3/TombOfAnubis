using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.PlayerCharacter
{
    internal class InputController
    {

        Keys[] upKeys = new Keys[] { Keys.W};
        Keys[] leftKeys = new Keys[] {Keys.A };
        Keys[] downKeys = new Keys[] {Keys.S };
        Keys[] rightKeys = new Keys[] {Keys.D };
        Keys[] useKeys = new Keys[] {Keys.E };

        Buttons[] upButtons = new Buttons[] { Buttons.DPadUp };
        Buttons[] leftButtons = new Buttons[] { Buttons.DPadLeft };
        Buttons[] downButtons = new Buttons[] { Buttons.DPadDown };
        Buttons[] rightButtons = new Buttons[] { Buttons.DPadRight };
        Buttons[] useButtons = new Buttons[] { Buttons.A };

        public InputController() { }

        //convert key presses by the current player into actions (walking left, right, up down, use, ...)
        public PlayerActions[] GetActionsOfCurrentPlayer(int player)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            GamePadState gamepadState = GamePadState.Default;

            if (player == 1) gamepadState = GamePad.GetState(PlayerIndex.One);
            if (player == 2) gamepadState = GamePad.GetState(PlayerIndex.Two);
            if (player == 3) gamepadState = GamePad.GetState(PlayerIndex.Three); 
            if (player == 4) gamepadState = GamePad.GetState(PlayerIndex.Four);

            Buttons[] pressedButtons = GetPressedButtons(gamepadState);

            List<PlayerActions> actions = new List<PlayerActions>();

            int playerIndex = player - 1;

            foreach(Keys key in pressedKeys)
            {
                if (key == upKeys[playerIndex])
                {
                    actions.Add(PlayerActions.WalkUp);
                }

                if (key == leftKeys[playerIndex])
                {
                    actions.Add(PlayerActions.WalkLeft);
                }

                if (key == downKeys[playerIndex])
                {
                    actions.Add(PlayerActions.WalkDown);
                }

                if (key == rightKeys[playerIndex])
                {
                    actions.Add(PlayerActions.WalkRight);
                }

                if (key == useKeys[playerIndex])
                {
                    actions.Add(PlayerActions.UseObject);
                }
            }

            foreach(Buttons button in pressedButtons)
            {
                if (button == upButtons[playerIndex])
                {
                    actions.Add(PlayerActions.WalkUp);
                }

                if (button == leftButtons[playerIndex])
                {
                    actions.Add(PlayerActions.WalkLeft);
                }

                if (button == downButtons[playerIndex])
                {
                    actions.Add(PlayerActions.WalkDown);
                }

                if (button == rightButtons[playerIndex])
                {
                    actions.Add(PlayerActions.WalkRight);
                }

                if (button == useButtons[playerIndex])
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
        public Buttons[] GetPressedButtons(GamePadState gamepadState)
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

        public List<PlayerActions> HandleOpposingActions(List<PlayerActions> actions)
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
