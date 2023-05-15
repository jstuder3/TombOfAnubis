#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TombOfAnubis
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    /// <remarks>
    /// Similar to a class found in the Game State Management sample on the 
    /// XNA Creators Club Online website (http://creators.xna.com).
    /// </remarks>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        protected int selectedEntry = 0;

        #endregion


        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        protected MenuEntry SelectedMenuEntry
        {
            get
            {
                if ((selectedEntry < 0) || (selectedEntry >= menuEntries.Count))
                {
                    return null;
                }
                return menuEntries[selectedEntry];
            }
        }

        protected bool buttonPressed;
        protected bool buttonCooldown;
        protected TimeSpan lastPressed;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            buttonCooldown = false;
            buttonPressed = false;
            lastPressed = TimeSpan.Zero;
        }


        #endregion


        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
        {
            if (!buttonCooldown)
            {
                // Move to the previous menu entry
                if (InputController.IsUpTriggered())
                {
                    buttonPressed = true;
                    AudioController.PlaySoundEffect("menuSelect");
                    selectedEntry--;
                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                }

                // Move to the next menu entry
                if (InputController.IsDownTriggered())
                {
                    buttonPressed = true;
                    AudioController.PlaySoundEffect("menuSelect");
                    selectedEntry = (selectedEntry + 1) % menuEntries.Count;
                }

                // Button pressed
                if (InputController.IsUseTriggered())
                {
                    buttonPressed = true;
                    AudioController.PlaySoundEffect("menuAccept");

                    foreach (PlayerInput playerInput in InputController.GetActiveInputs())
                    {
                        if (playerInput.IsKeyboard) InputController.KeyCooldowns.Add(playerInput.UseKey, 250);
                        else InputController.ButtonCooldowns.Add(playerInput.UseButton, 250);
                    }

                    OnSelectEntry(selectedEntry);

                }
            }

        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if(ScreenState == ScreenState.Hidden)
            {
                buttonPressed = true;
            }
            
            // Update button cooldown.
            if (buttonPressed)
            {
                buttonPressed = false;
                buttonCooldown = true;
                lastPressed = gameTime.TotalGameTime;
            }
            if (buttonCooldown) // enough time elapsed since last pressed
            {
                TimeSpan diff = gameTime.TotalGameTime - lastPressed;
                if (diff.TotalMilliseconds > 250)
                {
                    buttonCooldown = false;
                }
            }

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);
                //Debug.WriteLine("isActive: " + IsActive);
                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            spriteBatch.End();
        }


        #endregion
    }
}
