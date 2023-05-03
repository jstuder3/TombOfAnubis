using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace TombOfAnubis
{
    public enum PlayerAction
    {
        UseObject,
        PauseGame,
        DropObject
    }

    public class PlayerInput
    {
        public bool IsKeyboard { get; }
        public bool IsActive { get; set; }
        public int PlayerID { get; set; }
        public int ControllerID { get; set; }
        public Keys UpKey { get; set; }
        public Keys DownKey { get; set; }
        public Keys LeftKey { get; set; }
        public Keys RightKey { get; set; }
        public Keys UseKey { get; set; }
        public Keys PauseKey { get; set; }
        public Keys DropKey { get; set; }

        public Buttons UseButton { get; set; }
        public Buttons DropButton { get; set; }
        public Buttons PauseButton { get; set; }

        public PlayerInput(Keys up, Keys down, Keys left, Keys right, Keys use, Keys pause, Keys dropKey)
        {
            IsKeyboard = true;
            UpKey = up;
            DownKey = down;
            LeftKey = left;
            RightKey = right;
            UseKey = use;
            PauseKey = pause;
            DropKey = dropKey;
        }
        public PlayerInput(Buttons use, int controllerID)
        {
            ControllerID = controllerID;
            IsKeyboard = false;
            UseButton = use;
            PauseButton = Buttons.Start;
            DropButton = Buttons.B;
        }

        public void Update()
        {
            if (IsActive && IsKeyboard)
            {
                Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
                foreach (Keys key in pressedKeys)
                {
                    if (InputController.KeyCooldowns.ContainsKey(key)) { continue; }
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
                    if (key == PauseKey)
                    {
                        InputController.PlayerActions[PlayerID].Add(PlayerAction.PauseGame);
                    }
                    if (key == DropKey)
                    {
                        InputController.PlayerActions[PlayerID].Add(PlayerAction.DropObject);
                    }
                }
            }
            else if(IsActive)
            {
                GamePadState gamePadState = GamePad.GetState(ControllerID);
                InputController.PlayerMovementDirections[PlayerID] = gamePadState.ThumbSticks.Left * new Vector2(1f, -1f);

                InputController.PlayerMovementDirections[PlayerID] += (float)(gamePadState.DPad.Left) * new Vector2(-1, 0);
                InputController.PlayerMovementDirections[PlayerID] += (float)(gamePadState.DPad.Right) * new Vector2(1, 0);
                InputController.PlayerMovementDirections[PlayerID] += (float)(gamePadState.DPad.Up) * new Vector2(0, -1);
                InputController.PlayerMovementDirections[PlayerID] += (float)(gamePadState.DPad.Down) * new Vector2(0, 1);

                InputController.PlayerMovementDirections[PlayerID].Normalize();

                if (gamePadState.IsButtonDown(UseButton) && !InputController.ButtonCooldowns.ContainsKey(UseButton))
                {
                    InputController.PlayerActions[PlayerID].Add(PlayerAction.UseObject);
                }
                if (gamePadState.IsButtonDown(PauseButton) && !InputController.ButtonCooldowns.ContainsKey(PauseButton))
                {
                    InputController.PlayerActions[PlayerID].Add(PlayerAction.PauseGame);
                }
                if(gamePadState.IsButtonDown(DropButton) && !InputController.ButtonCooldowns.ContainsKey(DropButton))
                {
                    InputController.PlayerActions[PlayerID].Add(PlayerAction.DropObject);
                }
            }
        }

        public bool UseTriggered()
        {
            if (IsKeyboard)
            {
                return Keyboard.GetState().GetPressedKeys().Contains(UseKey);
            }
            else
            {
                return GamePad.GetState(ControllerID).IsButtonDown(UseButton);
            }
        }

        public void SetInactive()
        {
            IsActive = false;
            PlayerID = default(int);
        }
    }
    public static class InputController
    {
        public static PlayerInput[] PlayerInputs = new PlayerInput[] { 
            new PlayerInput(Keys.W, Keys.S, Keys.A, Keys.D, Keys.E, Keys.Escape, Keys.Q),
            new PlayerInput(Keys.T, Keys.G, Keys.F, Keys.H, Keys.Z, Keys.Escape, Keys.R),
            new PlayerInput(Keys.I, Keys.K, Keys.J, Keys.L, Keys.O, Keys.Escape, Keys.U),
            new PlayerInput(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.OemMinus, Keys.Escape, Keys.OemPeriod),
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

        public static Dictionary<Keys, int> KeyCooldowns = new Dictionary<Keys, int>();
        public static Dictionary<Buttons, int> ButtonCooldowns = new Dictionary<Buttons, int>();

        public static void Update(GameTime gameTime)
        {
            Clear();
            UpdateCooldowns(gameTime);
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

        public static void ResetPlayerInputs()
        {
            foreach(PlayerInput playerInput in PlayerInputs)
            {
                playerInput.SetInactive();
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

        public static bool IsPauseTriggered()
        {
            foreach (HashSet<PlayerAction> playerAction in PlayerActions)
            {
                if (playerAction.Contains(PlayerAction.PauseGame))
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

        public static void AddCooldown(Keys keys, Buttons button, int timeInMs)
        {
            KeyCooldowns.Add(keys, timeInMs);
            ButtonCooldowns.Add(button, timeInMs);
        }
        private static void UpdateCooldowns(GameTime gameTime)
        {
            foreach(Keys key in KeyCooldowns.Keys)
            {
                KeyCooldowns[key] -= gameTime.ElapsedGameTime.Milliseconds;
                if (KeyCooldowns[key] < 0)
                {
                    KeyCooldowns.Remove(key);
                }
            }
            foreach (Buttons button in ButtonCooldowns.Keys)
            {
                ButtonCooldowns[button] -= gameTime.ElapsedGameTime.Milliseconds;
                if (ButtonCooldowns[button] < 0)
                {
                    ButtonCooldowns.Remove(button);
                }
            }
        }
    }
}
