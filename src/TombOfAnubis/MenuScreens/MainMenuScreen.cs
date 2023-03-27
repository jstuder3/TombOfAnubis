#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace TombOfAnubis
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Graphics Data


        private Texture2D backgroundTexture;
        private Vector2 backgroundPosition;

        private Texture2D descriptionAreaTexture;
        private Vector2 descriptionAreaPosition;
        private Vector2 descriptionAreaTextPosition;

        private Texture2D iconTexture;
        private Vector2 iconPosition;

        private Texture2D backTexture;
        private Vector2 backPosition;

        private Texture2D selectTexture;
        private Vector2 selectPosition;

        private Texture2D plankTexture1, plankTexture2, plankTexture3;

        private int numberOfPlayers = 4;

        #endregion


        #region Menu Entries


        MenuEntry newGameMenuEntry, exitGameMenuEntry;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base()
        {
            // add the New Game entry
            newGameMenuEntry = new MenuEntry("New Game");
            newGameMenuEntry.Description = "Start a New Game";
            newGameMenuEntry.Font = Fonts.ArcheologicapsFont;
            newGameMenuEntry.Position = new Vector2(715, 0f);
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            MenuEntries.Add(newGameMenuEntry);

            // create the Exit menu entry
            exitGameMenuEntry = new MenuEntry("Exit");
            exitGameMenuEntry.Description = "Quit the Game";
            exitGameMenuEntry.Font = Fonts.ArcheologicapsFont;
            exitGameMenuEntry.Position = new Vector2(720, 0f);
            exitGameMenuEntry.Selected += OnCancel;
            MenuEntries.Add(exitGameMenuEntry);

            // TODO: Add number of players selection

            // start the menu music
            // AudioManager.PushMusic("MainTheme");
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {

            // TODO: Add actual textures
            // load the textures
            ContentManager content = GameScreenManager.Game.Content;
            backgroundTexture = content.Load<Texture2D>("Textures/Menu/bg");
            descriptionAreaTexture =
                content.Load<Texture2D>("Textures/Menu/0");
            iconTexture = content.Load<Texture2D>("Textures/Menu/1");
            plankTexture1 =
                content.Load<Texture2D>("Textures/Menu/2");
            plankTexture2 =
                content.Load<Texture2D>("Textures/Menu/2");
            plankTexture3 =
                content.Load<Texture2D>("Textures/Menu/3");
            backTexture = content.Load<Texture2D>("Textures/Menu/4");
            selectTexture = content.Load<Texture2D>("Textures/Menu/4");

            // calculate the texture positions
            Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;
            backgroundPosition = new Vector2(
                (viewport.Width - backgroundTexture.Width) / 2,
                (viewport.Height - backgroundTexture.Height) / 2);
            descriptionAreaPosition = backgroundPosition + new Vector2(158, 130);
            descriptionAreaTextPosition = backgroundPosition + new Vector2(158, 350);
            iconPosition = backgroundPosition + new Vector2(170, 80);
            backPosition = backgroundPosition + new Vector2(225, 610);
            selectPosition = backgroundPosition + new Vector2(1120, 610);

            // set the textures on each menu entry
            newGameMenuEntry.Texture = plankTexture3;
            exitGameMenuEntry.Texture = plankTexture1;

            // now that they have textures, set the proper positions on the menu entries
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntries[i].Position = new Vector2(
                    MenuEntries[i].Position.X,
                    500f - ((MenuEntries[i].Texture.Height - 10) *
                        (MenuEntries.Count - 1 - i)));
            }

            base.LoadContent();
        }

        #endregion


        #region Updating


        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            base.HandleInput();
        }


        /// <summary>
        /// Event handler for when the New Game menu entry is selected.
        /// </summary>
        void NewGameMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }

            ContentManager content = GameScreenManager.Game.Content;
            GameStartDescription gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map001";
            gameStartDescription.NumberOfPlayers = numberOfPlayers;
            LoadingScreen.Load(GameScreenManager, true, new GameplayScreen(gameStartDescription));
        }

        /// <summary>
        /// When the user cancels the main menu,
        /// or when the Exit Game menu entry is selected.
        /// </summary>
        protected override void OnCancel()
        {
            // add a confirmation message box
            string message = String.Empty;
            if (Session.IsActive)
            {
                message =
                    "Are you sure you want to exit?  All progress will be lost.";
            }
            else
            {
                message = "Are you sure you want to exit?";
            }
            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            GameScreenManager.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects Yes 
        /// on the "Are you sure?" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            GameScreenManager.Game.Exit();
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw this screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Main Menu work in progress

            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // draw the background images
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);
            spriteBatch.Draw(descriptionAreaTexture, descriptionAreaPosition,
                Color.White);
            spriteBatch.Draw(iconTexture, iconPosition, Color.White);

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntry.Draw(this, isSelected, gameTime);
            }

            // draw the description text for the selected entry
            MenuEntry selectedMenuEntry = SelectedMenuEntry;
            if ((selectedMenuEntry != null) &&
                !String.IsNullOrEmpty(selectedMenuEntry.Description))
            {
                Vector2 textSize =
                    Fonts.ArcheologicapsFont.MeasureString(selectedMenuEntry.Description);
                Vector2 textPosition = descriptionAreaTextPosition + new Vector2(
                    (float)Math.Floor((descriptionAreaTexture.Width - textSize.X) / 2),
                    0f);
                spriteBatch.DrawString(Fonts.ArcheologicapsFont,
                    selectedMenuEntry.Description, textPosition, Color.White);
            }

            // draw the select instruction
            spriteBatch.Draw(selectTexture, selectPosition, Color.White);
            spriteBatch.DrawString(Fonts.ArcheologicapsFont, "Select",
                new Vector2(
                selectPosition.X - Fonts.ArcheologicapsFont.MeasureString("Select").X - 5,
                selectPosition.Y + 5), Color.White);

            // if we are in-game, draw the back instruction
            if (Session.IsActive)
            {
                spriteBatch.Draw(backTexture, backPosition, Color.White);
                spriteBatch.DrawString(Fonts.ArcheologicapsFont, "Resume",
                    new Vector2(backPosition.X + 55, backPosition.Y + 5), Color.White);
            }

            spriteBatch.End();
        }

        #endregion
    }
}
