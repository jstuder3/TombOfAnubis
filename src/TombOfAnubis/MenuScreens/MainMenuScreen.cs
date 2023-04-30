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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Sdcb.FFmpeg.Raw;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using TombOfAnubis.GameScreens;
using TombOfAnubis.MenuScreens;
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
        private Rectangle backgroundPosition;

        private Vector2 titlePosition;
        private Texture2D titleTexture;
        private float titleScale = 0.6f;

        private SpriteFont menuFont;

        private Texture2D scrollTexture;
        private float scrollTextureScale = 0.4f;
        private static List<AnimationClip> activeScrollAnimation;
        private int scrollTextureWidth = 800, scrollTextureHeight = 400;

        private float marginX = 0.25f, marginY = 0.04f;

        #endregion

        #region QuitConfirmationDialog


        private bool dialogVisible = false;
        private bool confirmationStatus = false;
        private Color activeColor = Color.Red;
        private Color inactiveColor = Color.White;

        private Texture2D dialogTexture;
        private float dialogTransparency = 1.0f;
        private Rectangle dialogDimension;

        private Texture2D buttonTexture;
        private float buttonTransparency = 0.0f;
        private int dialogWidth = 800, dialogHeight = 400;
        private float marginTop = 0.35f, marginBottom = 0.3f, buttonSpacing = 0.15f;
        private int buttonOptionWidth = 150, buttonOptionHeight = 50;

        #endregion


        #region Menu Entries


        MenuEntry newGameMenuEntry, settingsMenuEntry, quitMenuEntry;

       
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base()
        {
            buttonPressed = true;

            gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map001";
            gameStartDescription.NumberOfPlayers = InputController.GetActiveInputs().Count;

            // Add the New Game entry
            newGameMenuEntry = new MenuEntry("New Game");
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            MenuEntries.Add(newGameMenuEntry);

            // Create the  Settings entry
            settingsMenuEntry = new MenuEntry("Settings");
            settingsMenuEntry.Selected += SettingsMenuEntrySelected;
            MenuEntries.Add(settingsMenuEntry);

            // Create the Quit game entry
            quitMenuEntry = new MenuEntry("Quit");
            quitMenuEntry.Selected += QuitMenuEntrySelected;
            MenuEntries.Add(quitMenuEntry);
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            if(!AudioController.isPlayingSong())
                AudioController.PlaySong("menuScreen");

            // Load the textures
            ContentManager content = GameScreenManager.Game.Content;
            backgroundTexture = content.Load<Texture2D>("Textures/Menu/main_menu_bg");
            scrollTexture = content.Load<Texture2D>("Textures/Menu/Scroll");
            titleTexture = content.Load<Texture2D>("Textures/Menu/Title_advanced");

            menuFont = Fonts.DisneyHeroicFont;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Font = menuFont;
            }

            activeScrollAnimation = new List<AnimationClip> {
                            new AnimationClip(AnimationClipType.InactiveEntry, 1, 50, new Point(scrollTextureWidth, scrollTextureHeight)),
                            new AnimationClip(AnimationClipType.TransitionEntry, 3, 30, new Point(scrollTextureWidth, scrollTextureHeight)),
                            new AnimationClip(AnimationClipType.ActiveEntry, 1, 50, new Point(scrollTextureWidth, scrollTextureHeight)),
                        };

            // Set the textures on each menu element and its scale
            Animation animation = new Animation(activeScrollAnimation, Visibility.Game);
            SetAnimation(scrollTexture, scrollTextureScale, animation);

            // Now that they have textures, set the proper positions on the menu entries
            Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;
            SetElementPosition(viewport);

            // Dialog Content
            dialogTexture = scrollTexture;
            buttonTexture = new Texture2D(GameScreenManager.GraphicsDevice, 1, 1);
            buttonTexture.SetData(new[] { Color.Gray });

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
            // Main Screen

            int screenWidth = viewport.Width;
            int screenHeight = viewport.Height;

            backgroundPosition = new Rectangle(0, 0, screenWidth, screenHeight);

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


            // Quit Confirmation Dialog

            int posOffsetX = (int)(screenWidth - dialogWidth) / 2;
            int posOffsetY = (int)(screenHeight - dialogHeight) / 2;
            dialogDimension = new Rectangle(posOffsetX, posOffsetY, dialogWidth, dialogHeight);
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
            //LoadingScreen.Load(GameScreenManager, true, new LoginPlayersScreen());
            GameScreenManager.AddScreen(new LoginPlayersScreen());
        }

        /// <summary>
        /// Event handler for when the Controls menu entry is selected.
        /// </summary>
        void SettingsMenuEntrySelected(object sender, EventArgs e)
        {
            GameScreenManager.AddScreen(new SettingsScreen());
        }

        /// <summary>
        /// Event handler for when the Credits menu entry is selected.
        /// </summary>
        void QuitMenuEntrySelected(object sender, EventArgs e)
        {
            dialogVisible = true;
        }

        #endregion

        #region HandleInput

        public override void HandleInput()
        {
            if(dialogVisible)
            {
                HandleQuitDialogInput();
            }
            else
            {
                base.HandleInput();
            }
        }

        private void HandleQuitDialogInput()
        {
            if (!buttonCooldown)
            {
                if (InputController.IsleftTriggered())
                {
                    if (!confirmationStatus) { AudioController.PlaySoundEffect("menuSelect"); }
                    buttonPressed = true;
                    confirmationStatus = true;

                }

                if (InputController.IsRightTriggered())
                {
                    if (confirmationStatus) { AudioController.PlaySoundEffect("menuSelect"); }
                    buttonPressed = true;
                    confirmationStatus = false;
                }

                if (InputController.IsUseTriggered())
                {
                    AudioController.PlaySoundEffect("menuAccept");
                    if (confirmationStatus)
                    {
                        GameScreenManager.Game.Exit();
                    }
                    else
                    {
                        dialogVisible = false;
                        buttonPressed = true;
                        confirmationStatus = false;
                    }
                }
            }
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
                menuEntry.Draw(this, isSelected);
            }

            if (dialogVisible) DrawDialog();

            spriteBatch.End();
        }

        void DrawDialog()
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;
            //spriteBatch.Draw(dialogTexture, dialogDimension, Color.White * dialogTransparency);
            Rectangle sourceRectangle = new Rectangle(0, 800, 800, 400);
            spriteBatch.Draw(dialogTexture, dialogDimension, sourceRectangle, Color.White * dialogTransparency);

            string confirmationMessage = "Are you sure you want to quit?";
            string positive = "Yes", negative = "No";

            // Draw Confirmation message
            Vector2 textDim = menuFont.MeasureString(confirmationMessage);
            int posOffsetX = (int)(dialogDimension.X + (dialogWidth - textDim.X) / 2);
            //int posOffsetY = (int)(dialogDimension.Y + (dialogHeight - textDim.Y) / 2);
            int posOffsetY = (int)(dialogDimension.Y + marginTop * dialogHeight);
            Vector2 textPosition = new Vector2(posOffsetX, posOffsetY);
            spriteBatch.DrawString(menuFont, confirmationMessage, textPosition, Color.White);

            // Draw Yes Button
            int buttonOffsetX = (int)(dialogDimension.X + ((1 - buttonSpacing) * dialogWidth - 2 * buttonOptionWidth) / 2);
            int buttonOffsetY = (int)(dialogDimension.Y + (1 - marginBottom) * dialogHeight - buttonOptionHeight);
            Rectangle yesButtonPosition = new Rectangle(buttonOffsetX, buttonOffsetY, buttonOptionWidth, buttonOptionHeight);
            textDim = menuFont.MeasureString(positive);
            posOffsetX = (int)(buttonOffsetX + (buttonOptionWidth - textDim.X) / 2);
            posOffsetY = (int)(buttonOffsetY + (buttonOptionHeight - textDim.Y) / 2);
            textPosition = new Vector2(posOffsetX, posOffsetY);
            spriteBatch.Draw(buttonTexture, yesButtonPosition, Color.White * buttonTransparency);
            Color textColor = confirmationStatus ? activeColor : inactiveColor;
            spriteBatch.DrawString(menuFont, positive, textPosition, textColor);

            // Draw No Button
            buttonOffsetX = (int)(buttonOffsetX + buttonOptionWidth + buttonSpacing * dialogWidth);
            Rectangle noButtonPosition = new Rectangle(buttonOffsetX, buttonOffsetY, buttonOptionWidth, buttonOptionHeight);
            textDim = menuFont.MeasureString(negative);
            posOffsetX = (int)(buttonOffsetX + (buttonOptionWidth - textDim.X) / 2);
            posOffsetY = (int)(buttonOffsetY + (buttonOptionHeight - textDim.Y) / 2);
            textPosition = new Vector2(posOffsetX, posOffsetY);
            spriteBatch.Draw(buttonTexture, noButtonPosition, Color.White * buttonTransparency);
            textColor = !confirmationStatus ? activeColor : inactiveColor;
            spriteBatch.DrawString(menuFont, negative, textPosition, textColor);
        }

        #endregion
    }
}
