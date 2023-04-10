using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TombOfAnubis
{
    public enum PlayerAction
    {
        UseObject,
        UseBodyPowerup,
        UseWisdomPowerup
    }
    public static class InputController
    {

        public static Keys[] UpKeys = new Keys[] { Keys.W, Keys.T, Keys.I, Keys.Up };
        public static Keys[] LeftKeys = new Keys[] { Keys.A, Keys.F, Keys.J, Keys.Left };
        public static Keys[] DownKeys = new Keys[] { Keys.S, Keys.G, Keys.K, Keys.Down };
        public static Keys[] RightKeys = new Keys[] { Keys.D, Keys.H, Keys.L, Keys.Right };
        public static Keys[] UseKeys = new Keys[] { Keys.E, Keys.Z, Keys.O, Keys.OemMinus };
        // public static Keys[] BodyPowerupKeys = new Keys[] { Keys.E, Keys.Z, Keys.O, Keys.OemMinus };
        // public static Keys[] WisdomPowerupKeys = new Keys[] { Keys.Q, Keys.R, Keys.U, Keys.OemPeriod };

        public static Buttons[] UseButtons = new Buttons[] { Buttons.A, Buttons.A, Buttons.A, Buttons.A };

        public static Vector2[] PlayerMovementDirections = new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };

        public static HashSet<PlayerAction>[] PlayerActions = new HashSet<PlayerAction>[] { new HashSet<PlayerAction>(), new HashSet<PlayerAction>(), new HashSet<PlayerAction>() , new HashSet<PlayerAction>() };
        // public static Buttons[] BodyPowerupButtons = new Buttons[] { Buttons.B, Buttons.B, Buttons.B, Buttons.B };
        // public static Buttons[] WisdomPowerupButtons = new Buttons[] { Buttons.X, Buttons.X, Buttons.X, Buttons.X };

        public static readonly Vector2 UP = new Vector2(0, -1f);
        public static readonly Vector2 DOWN = new Vector2(0, 1f);
        public static readonly Vector2 LEFT = new Vector2(-1f, 0);
        public static readonly Vector2 RIGHT = new Vector2(1f, 0);

        public static void Update()
        {
            Clear();
            for(int playerId = 0; playerId < 4; playerId++)
            {

                GamePadState gamepadState = GamePad.GetState(playerId);

                PlayerMovementDirections[playerId] = gamepadState.ThumbSticks.Left * new Vector2(1f, -1f);

                bool keyboardInput = false;
                Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
                foreach (Keys key in pressedKeys)
                {
                    if (key == UpKeys[playerId])
                    {
                        PlayerMovementDirections[playerId].Y += -1f;
                        keyboardInput = true;
                    }

                    if (key == LeftKeys[playerId])
                    {
                        PlayerMovementDirections[playerId].X += -1f;
                        keyboardInput = true;
                    }

                    if (key == DownKeys[playerId])
                    {
                        PlayerMovementDirections[playerId].Y += 1f;
                        keyboardInput = true;
                    }

                    if (key == RightKeys[playerId])
                    {
                        PlayerMovementDirections[playerId].X += 1f;
                        keyboardInput = true;
                    }
                    if (keyboardInput == true && PlayerMovementDirections[playerId].Length() > 0)
                    {
                        PlayerMovementDirections[playerId].Normalize();
                    }
                    if (key == UseKeys[playerId])
                    {
                        PlayerActions[playerId].Add(PlayerAction.UseObject);
                    }

                }
                Buttons[] pressedButtons = GetPressedButtons(gamepadState);
                foreach (Buttons button in pressedButtons)
                {
                    if (button == UseButtons[playerId])
                    {
                        PlayerActions[playerId].Add(PlayerAction.UseObject);
                    }
                }
            }
        }
        public static void Clear()
        {
            for (int playerId = 0; playerId < 4; playerId++)
            {
                PlayerMovementDirections[playerId] = Vector2.Zero;
                PlayerActions[playerId].Clear();
            }
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
        public static bool IsDownTriggered()
        {
            for (int playerId = 0; playerId < PlayerMovementDirections.Length; playerId++)
            {
                if (Vector2.Dot(DOWN, PlayerMovementDirections[playerId]) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsUpTriggered()
        {
            for (int playerId = 0; playerId < PlayerMovementDirections.Length; playerId++)
            {
                if (Vector2.Dot(UP, PlayerMovementDirections[playerId]) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsRightTriggered()
        {
            for (int playerId = 0; playerId < PlayerMovementDirections.Length; playerId++)
            {
                if (Vector2.Dot(RIGHT, PlayerMovementDirections[playerId]) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsleftTriggered()
        {
            for (int playerId = 0; playerId < PlayerMovementDirections.Length; playerId++)
            {
                if (Vector2.Dot(LEFT, PlayerMovementDirections[playerId]) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsUseTriggered()
        {
            for (int playerId = 0; playerId < PlayerActions.Length; playerId++)
            {
                if (PlayerActions[playerId].Contains(PlayerAction.UseObject))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
