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
using Sdcb.FFmpeg.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using TombOfAnubis.GameScreens;
using TombOfAnubisContentData;
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

        private Vector2 titlePosition;
        private Texture2D titleTexture;
        private float titleScale = 0.3f;

        private Texture2D scrollTexture;
        private float scrollTextureScale = 0.4f;
        private static List<AnimationClip> activeScrollAnimation;
        private int scrollTextureWidth = 800, scrollTextureHeight = 400;

        private float marginX = 0.15f, marginY = 0.05f;

        #endregion


        #region Menu Entries


        MenuEntry newGameMenuEntry, controlsMenuEntry, creditsMenuEntry;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base()
        {
            gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map002";
            gameStartDescription.NumberOfPlayers = InputController.GetActiveInputs().Count;

            // Add the New Game entry
            newGameMenuEntry = new MenuEntry("New Game");
            newGameMenuEntry.Font = Fonts.DisneyHeroicFont;
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            MenuEntries.Add(newGameMenuEntry);

            // Create the Controls menu entry
            controlsMenuEntry = new MenuEntry("Controls");
            controlsMenuEntry.Font = Fonts.DisneyHeroicFont;
            controlsMenuEntry.Selected += ControlsMenuEntrySelected;
            MenuEntries.Add(controlsMenuEntry);

            // Create the Credits menu entry
            creditsMenuEntry = new MenuEntry("Credits");
            creditsMenuEntry.Font = Fonts.DisneyHeroicFont;
            creditsMenuEntry.Selected += CreditsMenuEntrySelected;
            MenuEntries.Add(creditsMenuEntry);
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {

            // Load the textures
            ContentManager content = GameScreenManager.Game.Content;
            backgroundTexture = content.Load<Texture2D>("Textures/Menu/plagiarized_bg");
            scrollTexture = content.Load<Texture2D>("Textures/Menu/Scroll");
            titleTexture = content.Load<Texture2D>("Textures/Menu/Title_white");

            activeScrollAnimation = new List<AnimationClip> {
                            new AnimationClip(AnimationClipType.InactiveEntry, 1, 50, new Point(scrollTextureWidth, scrollTextureHeight)),
                            new AnimationClip(AnimationClipType.TransitionEntry, 3, 30, new Point(scrollTextureWidth, scrollTextureHeight)),
                            new AnimationClip(AnimationClipType.ActiveEntry, 1, 50, new Point(scrollTextureWidth, scrollTextureHeight)),
                        };

            Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;

            // Set the textures on each menu element and its scale
            Animation animation = new Animation(activeScrollAnimation, Visibility.Game);
            SetAnimation(scrollTexture, scrollTextureScale, animation);

            // Now that they have textures, set the proper positions on the menu entries
            SetElementPosition(viewport);

            AudioController.PlaySong("background_music");
            base.LoadContent();
        }

        public void SetAnimation(Texture2D animationSpritesheet, float spriteScale, Animation animation)
        {
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Texture = animationSpritesheet;
                entry.TextureScale = spriteScale;
                entry.TextureAnimation = animation;
            }
        }

        public void SetElementPosition(Viewport viewport)
        {
            int screenWidth = viewport.Width;
            int screenHeight = viewport.Height;

            // Center background image around viewport
            backgroundPosition = new Vector2(
                (screenWidth - backgroundTexture.Width) / 2,
                (screenHeight - backgroundTexture.Height) / 2);

            float titleWidth = titleScale * titleTexture.Width / screenWidth;
            float titleHeight = titleScale * titleTexture.Height / screenHeight;
            // Assume every entry has the same sized texture
            float textureWidth = ((float) scrollTextureWidth / screenWidth) * scrollTextureScale;
            float textureHeight = ((float) scrollTextureHeight / screenHeight) * scrollTextureScale;

            float titleOffsetX = marginX + (textureWidth - titleWidth) / 2;
            titlePosition = GetRelativePosition(viewport, titleOffsetX, marginY);

            // The first MenuEntry element is drawn at this relative vertical coordinate
            float entryStart = titleHeight + 3 * marginY;

            for (int i = 0; i < MenuEntries.Count; i++)
            {
                float entrySpacing = i * textureHeight;

                float offsetY = entryStart + entrySpacing;

                MenuEntries[i].Position = GetRelativePosition(viewport, marginX, offsetY);
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


        #region Updating


        /// <summary>
        /// Event handler for when the New Game menu entry is selected.
        /// </summary>
        void NewGameMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }
            //LoadingScreen.Load(GameScreenManager, true, new GameplayScreen(gameStartDescription));
            GameScreenManager.AddScreen(new LoginPlayersScreen());
        }

        /// <summary>
        /// Event handler for when the Controls menu entry is selected.
        /// </summary>
        void ControlsMenuEntrySelected(object sender, EventArgs e)
        {
            controlsMenuEntry.Text = "Controls";
        }

        /// <summary>
        /// Event handler for when the Credits menu entry is selected.
        /// </summary>
        void CreditsMenuEntrySelected(object sender, EventArgs e)
        {
            creditsMenuEntry.Text = "Credits";
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
            spriteBatch.Draw(titleTexture, titlePosition, null, Color.White, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

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
