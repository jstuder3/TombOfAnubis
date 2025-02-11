﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    class GameOverScreen : MenuScreen
    {
        private GameStartDescription gameStartDescription;

        #region Graphics Data


        private Texture2D backgroundTexture;
        private Rectangle backgroundPosition;
        private SpriteFont menuFont;

        private Texture2D scrollTexture;
        private float scrollTextureScale = 0.5f;
        private int scrollTextureWidth = 800, scrollTextureHeight = 400;

        private float buttonSpacing = 0.1f, buttonOffsetY = 0.6f;

        private bool firstEntry;

        #endregion


        #region Menu Entries


        MenuEntry restartMenuEntry, endGameMenuEntry;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public GameOverScreen() : base()
        {
            buttonPressed = true;
            buttonCooldown = true;
            firstEntry = true;

            gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map001";
            gameStartDescription.NumberOfPlayers = InputController.GetActiveInputs().Count;

            // Add the New Game entry
            restartMenuEntry = new MenuEntry("Restart");
            restartMenuEntry.Selected += RestartMenuEntrySelected;
            MenuEntries.Add(restartMenuEntry);

            // Create the Exit menu entry
            endGameMenuEntry = new MenuEntry("Main Menu");
            endGameMenuEntry.Selected += EndGameMenuEntrySelected;
            MenuEntries.Add(endGameMenuEntry);
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {

            AudioController.StopSong();

            // Load the textures
            ContentManager content = GameScreenManager.Game.Content;
            if(InputController.GetActiveInputs().Count == 1)
            {
                backgroundTexture = content.Load<Texture2D>("Textures/Menu/GameOverBG_1Player");
            }
            else
            {
                backgroundTexture = content.Load<Texture2D>("Textures/Menu/GameOverBG");
            }
            scrollTexture = content.Load<Texture2D>("Textures/Menu/Scroll");

            menuFont = Fonts.DisneyHeroicFont;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Font = menuFont;
                entry.Texture = scrollTexture;
                entry.TextureScale = scrollTextureScale;
            }

            Viewport viewport = ResolutionController.TargetViewport;

            // Now that they have textures, set the proper positions on the menu entries
            SetElementPosition(viewport);

            base.LoadContent();
        }

        public void SetElementPosition(Viewport viewport)
        {
            int screenWidth = viewport.Width;
            int screenHeight = viewport.Height;
            int numButtons = MenuEntries.Count;

            // Center background image around viewport
            backgroundPosition = new Rectangle(0,0,screenWidth,screenHeight);

            // Assume every entry has the same sized texture
            float textureWidth = ((float)scrollTextureWidth / screenWidth) * scrollTextureScale;
            float textureHeight = ((float)scrollTextureHeight / screenHeight) * scrollTextureScale;

            // Center the UI element according to the screen width
            float textureOffsetX = (1.0f - numButtons * textureWidth - (numButtons - 1) * buttonSpacing) / 2;

            for (int i = 0; i < numButtons; i++)
            {
                float entrySpacing = i * (textureWidth + buttonSpacing);

                float offSetX = textureOffsetX + entrySpacing;

                MenuEntries[i].Position = GetRelativePosition(viewport, offSetX, buttonOffsetY);
            }
        }

        /// <summary>
        /// Returns the position on viewport relative to its width and height
        /// </summary>
        public Vector2 GetRelativePosition(Viewport viewport, float offsetX, float offsetY)
        {
            int xPos = (int)(viewport.Width * offsetX);
            int yPos = (int)(viewport.Height * offsetY);
            Vector2 relPosition = new Vector2(xPos, yPos);

            return relPosition;
        }

        #endregion

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
        {
            if (!buttonCooldown)
            {
                // Move to the previous menu entry
                if (InputController.IsleftTriggered())
                {
                    AudioController.PlaySoundEffect("menuSelect");
                    selectedEntry--;
                    if (selectedEntry < 0)
                        selectedEntry = 0;
                    buttonPressed = true;
                }

                // Move to the next menu entry
                if (InputController.IsRightTriggered())
                {
                    AudioController.PlaySoundEffect("menuSelect");
                    selectedEntry++;
                    if (selectedEntry > MenuEntries.Count - 1)
                        selectedEntry = MenuEntries.Count - 1;
                    buttonPressed = true;
                }

                // Button pressed
                if (InputController.IsUseTriggered())
                {
                    AudioController.PlaySoundEffect("menuAccept");
                    OnSelectEntry(selectedEntry);
                    buttonPressed = true;
                }
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                               bool coveredByOtherScreen)
        {
            if (ScreenState == ScreenState.Hidden)
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
                int cooldownPeriod;
                if (firstEntry) { cooldownPeriod = 750; }
                else { cooldownPeriod = 250; }

                TimeSpan diff = gameTime.TotalGameTime - lastPressed;
                if (diff.TotalMilliseconds > cooldownPeriod)
                {
                    firstEntry = false;
                    buttonCooldown = false;
                }
            }

            // Update each nested MenuEntry object.
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);
                //Debug.WriteLine("isActive: " + IsActive);
                MenuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        #region Updating

        /// <summary>
        /// Event handler for when the Restart menu entry is selected.
        /// </summary>
        void RestartMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }
            GameScreenManager.RemoveScreen(this);
            LoadingScreen.LoadAtRestart(GameScreenManager, true, new GameplayScreen(gameStartDescription));
        }

        /// <summary>
        /// Event handler for when the End Game menu entry is selected.
        /// </summary>
        void EndGameMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }
            //LoadingScreen.Load(GameScreenManager, true, new IntroScreen());
            RemoveSecondaryInputs();
            GameScreenManager.RemoveScreen(this);
            GameScreenManager.AddScreen(new MainMenuScreen());
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

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntry.Draw(this, isSelected, gameTime);
            }

            spriteBatch.End();
        }

        #endregion
    }
}
