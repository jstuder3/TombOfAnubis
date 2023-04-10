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

    public class PlayerInput
    {
        public bool IsKeyboard { get; }
        public bool IsActive { get; set; }
        public int PlayerID { get; set; }
        public int ControllerID { get; set; }
        private Keys UpKey;
        private Keys DownKey;
        private Keys LeftKey;
        private Keys RightKey;
        private Keys UseKey;

        private Buttons UseButton;

        public PlayerInput(Keys up, Keys down, Keys left, Keys right, Keys use) {
            IsKeyboard = true;
            UpKey = up;
            DownKey = down;
            LeftKey = left;
            RightKey = right;
            UseKey = use;
        }
        public PlayerInput(Buttons use, int controllerID)
        {
            ControllerID = controllerID;
            IsKeyboard = false;
            UseButton = use;
        }

        public void Update()
        {
            if (IsActive && IsKeyboard)
            {
                Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
                foreach (Keys key in pressedKeys)
                {
                    if (key == UpKey)
                    {
                        InputController.PlayerMovementDirections[PlayerID].Y += -1f;
                    }

                    if (key == LeftKey)
                    {
                        InputController.PlayerMovementDirections[PlayerID].X += -1f;
                    }

                    if (key == DownKey)
                    {
                        InputController.PlayerMovementDirections[PlayerID].Y += 1f;
                    }

                    if (key == RightKey)
                    {
                        InputController.PlayerMovementDirections[PlayerID].X += 1f;
                    }
                    if (InputController.PlayerMovementDirections[PlayerID].Length() > 0)
                    {
                        InputController.PlayerMovementDirections[PlayerID].Normalize();
                    }
                    if (key == UseKey)
                    {
                        InputController.PlayerActions[PlayerID].Add(PlayerAction.UseObject);
                    }
                }
            }
            else if(IsActive)
            {
                GamePadState gamePadState = GamePad.GetState(ControllerID);
                InputController.PlayerMovementDirections[PlayerID] = gamePadState.ThumbSticks.Left * new Vector2(1f, -1f);
                if (gamePadState.IsButtonDown(UseButton))
                {
                    InputController.PlayerActions[PlayerID].Add(PlayerAction.UseObject);
                }
            }
        }

        public bool UseTriggered()
        {
            if (IsKeyboard)
            {
                return Keyboard.GetState().IsKeyDown(UseKey);
            }
            else
            {
                return GamePad.GetState(ControllerID).IsButtonDown(UseButton);
            }
        }
    }
    public static class InputController
    {
        public static PlayerInput[] PlayerInputs = new PlayerInput[] { 
            new PlayerInput(Keys.W, Keys.S, Keys.A, Keys.D, Keys.E),
            new PlayerInput(Keys.T, Keys.G, Keys.F, Keys.H, Keys.Z),
            new PlayerInput(Keys.I, Keys.K, Keys.J, Keys.L, Keys.O),
            new PlayerInput(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.OemMinus),
            new PlayerInput(Buttons.A, 0),
            new PlayerInput(Buttons.A, 1),
            new PlayerInput(Buttons.A, 2),
            new PlayerInput(Buttons.A, 3),
        };

        public static Vector2[] PlayerMovementDirections = new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };

        public static HashSet<PlayerAction>[] PlayerActions = new HashSet<PlayerAction>[] {
            new HashSet<PlayerAction>(),
            new HashSet<PlayerAction>(), 
            new HashSet<PlayerAction>(), 
            new HashSet<PlayerAction>()};

        public static readonly Vector2 UP = new Vector2(0, -1f);
        public static readonly Vector2 DOWN = new Vector2(0, 1f);
        public static readonly Vector2 LEFT = new Vector2(-1f, 0);
        public static readonly Vector2 RIGHT = new Vector2(1f, 0);

        public static void Update()
        {
            Clear();
            foreach(PlayerInput playerInput in PlayerInputs)
            {
                playerInput.Update();
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

        public static bool IsDownTriggered()
        {
            foreach (Vector2 dir in PlayerMovementDirections)
            {
                if (Vector2.Dot(DOWN, dir) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsUpTriggered()
        {
            foreach (Vector2 dir in PlayerMovementDirections)
            {
                if (Vector2.Dot(UP, dir) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsRightTriggered()
        {
            foreach (Vector2 dir in PlayerMovementDirections)
            {
                if (Vector2.Dot(RIGHT, dir) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsleftTriggered()
        {
            foreach(Vector2 dir in PlayerMovementDirections)
            {
                if (Vector2.Dot(LEFT, dir) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsUseTriggered()
        {
            foreach(HashSet<PlayerAction> playerAction in PlayerActions)
            {
                if(playerAction.Contains(PlayerAction.UseObject))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<PlayerInput> GetActiveInputs()
        {
            List<PlayerInput> res = new List<PlayerInput>();
            foreach (PlayerInput input in PlayerInputs)
            {
                if(input.IsActive)
                {
                    res.Add(input);
                }
            }
            res.Sort((x, y) => x.PlayerID - y.PlayerID);
            return res;
        }
    }
}
