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
using System.Linq;
#endregion

namespace TombOfAnubis
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        private GameStartDescription gameStartDescription;

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

        #endregion


        #region Menu Entries


        MenuEntry newGameMenuEntry, playerSelectionMenuEntry;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base()
        {
            gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map001";
            gameStartDescription.NumberOfPlayers = 0;

            // add the New Game entry
            newGameMenuEntry = new MenuEntry("New Game");
            newGameMenuEntry.Description = "Start a New Game (Press E)";
            newGameMenuEntry.Font = Fonts.DisneyHeroicFont;
            newGameMenuEntry.Position = new Vector2(715, 0f);
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            MenuEntries.Add(newGameMenuEntry);

            // create the Exit menu entry
            playerSelectionMenuEntry = new MenuEntry("Number of players: " + gameStartDescription.NumberOfPlayers);
            playerSelectionMenuEntry.Description = "Choose the number of players";
            playerSelectionMenuEntry.Font = Fonts.DisneyHeroicFont;
            playerSelectionMenuEntry.Position = new Vector2(720, 0f);
            playerSelectionMenuEntry.Selected += PlayerSelectionMenuEntrySelected;
            MenuEntries.Add(playerSelectionMenuEntry);

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
            backgroundTexture = content.Load<Texture2D>("Textures/Menu/plagiarized_bg");
            descriptionAreaTexture =
                content.Load<Texture2D>("Textures/Menu/MenuTile");
            iconTexture = content.Load<Texture2D>("Textures/Menu/MenuTile");
            plankTexture1 =
                content.Load<Texture2D>("Textures/Menu/MenuTile");
            plankTexture2 =
                content.Load<Texture2D>("Textures/Menu/MenuTile");
            plankTexture3 =
                content.Load<Texture2D>("Textures/Menu/MenuTile");
            backTexture = content.Load<Texture2D>("Textures/Menu/MenuTile");
            selectTexture = content.Load<Texture2D>("Textures/Menu/MenuTile");

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
            playerSelectionMenuEntry.Texture = plankTexture1;

            // now that they have textures, set the proper positions on the menu entries
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntries[i].Position = new Vector2(
                    MenuEntries[i].Position.X,
                    500f - ((MenuEntries[i].Texture.Height - 10) *
                        (MenuEntries.Count - 1 - i)));
            }
            AudioController.PlaySong("background_music");
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
            foreach (PlayerInput playerInput in InputController.PlayerInputs)
            {
                if (playerInput.UseTriggered() && !playerInput.IsActive)
                {
                    int activeInputs = InputController.GetActiveInputs().Count();
                    if (activeInputs < 4)
                    {
                        playerInput.IsActive = true;
                        playerInput.PlayerID = activeInputs;
                        buttonPressed = true;
                        gameStartDescription.NumberOfPlayers = activeInputs + 1;
                        playerSelectionMenuEntry.Text = "Number of players: " + gameStartDescription.NumberOfPlayers;
                    }
                }
            }
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
            LoadingScreen.Load(GameScreenManager, true, new GameplayScreen(gameStartDescription));
        }

        /// <summary>
        /// Event handler for when the Number of players menu entry is selected.
        /// </summary>
        void PlayerSelectionMenuEntrySelected(object sender, EventArgs e)
        {
            //gameStartDescription.NumberOfPlayers = (gameStartDescription.NumberOfPlayers %4)+1;
            playerSelectionMenuEntry.Text = "Number of players: " + gameStartDescription.NumberOfPlayers;
        }


        /// <summary>
        /// Event handler for when the user selects Yes 
        /// on the "Are you sure?" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            //GameScreenManager.Game.Exit();
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw this screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

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
                    Fonts.DisneyHeroicFont.MeasureString(selectedMenuEntry.Description);
                Vector2 textPosition = descriptionAreaTextPosition + new Vector2(
                    (float)Math.Floor((descriptionAreaTexture.Width - textSize.X) / 2),
                    0f);
                spriteBatch.DrawString(Fonts.DisneyHeroicFont,
                    selectedMenuEntry.Description, textPosition, Fonts.TitleColor);
            }
            /*
            // draw the select instruction
            spriteBatch.Draw(selectTexture, selectPosition, Color.White);
            spriteBatch.DrawString(Fonts.DisneyHeroicFont, "Select",
                new Vector2(
                selectPosition.X - Fonts.DisneyHeroicFont.MeasureString("Select").X - 5,
                selectPosition.Y + 5), Color.White);

            // if we are in-game, draw the back instruction
            if (Session.IsActive)
            {
                spriteBatch.Draw(backTexture, backPosition, Color.White);
                spriteBatch.DrawString(Fonts.DisneyHeroicFont, "Resume",
                    new Vector2(backPosition.X + 55, backPosition.Y + 5), Color.White);
            }
            */

            spriteBatch.End();
        }

        #endregion
    }
}
