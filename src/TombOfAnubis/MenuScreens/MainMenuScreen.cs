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
        private float titleScale = 0.4f;

        private Texture2D scrollTexture;
        private float scrollTextureScale = 0.5f;
        private static List<AnimationClip> activeScrollAnimation;
        private int scrollTextureWidth = 800, scrollTextureHeight = 400;

        private Texture2D emptyPlayerSlot;
        private Texture2D keyboardPlayerSlot, controllerPlayerSlot;
        private List<Color> playerColors = new List<Color> { Color.Red, Color.Green, Color.Blue, Color.Purple};
        private List<Texture2D> connectedPlayerSlots;
        private float slotScale = 0.6f;

        private float marginX = 0.05f, marginY = 0.05f;

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
            gameStartDescription.MapContentName = "Map002";
            gameStartDescription.NumberOfPlayers = InputController.GetActiveInputs().Count;

            // Add the New Game entry
            newGameMenuEntry = new MenuEntry("New Game");
            newGameMenuEntry.Font = Fonts.DisneyHeroicFont;
            newGameMenuEntry.Selected += NewGameMenuEntrySelected;
            MenuEntries.Add(newGameMenuEntry);

            // Create the Exit menu entry
            playerSelectionMenuEntry = new MenuEntry("Players connected");
            playerSelectionMenuEntry.Font = Fonts.DisneyHeroicFont;
            playerSelectionMenuEntry.Selected += PlayerSelectionMenuEntrySelected;
            MenuEntries.Add(playerSelectionMenuEntry);
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

            emptyPlayerSlot = content.Load<Texture2D>("Textures/Menu/Empty");
            keyboardPlayerSlot = content.Load<Texture2D>("Textures/Menu/Keyboard");
            controllerPlayerSlot = content.Load<Texture2D>("Textures/Menu/Controller");

            activeScrollAnimation = new List<AnimationClip> {
                            new AnimationClip(AnimationClipType.InactiveEntry, 1, 200, new Point(scrollTextureWidth, scrollTextureHeight)),
                            new AnimationClip(AnimationClipType.TransitionEntry, 3, 200, new Point(scrollTextureWidth, scrollTextureHeight)),
                            new AnimationClip(AnimationClipType.ActiveEntry, 1, 200, new Point(scrollTextureWidth, scrollTextureHeight)),
                        };

            Texture2D playerOneSlot = InputController.GetActiveInputs()[0].IsKeyboard ? keyboardPlayerSlot : controllerPlayerSlot;
            connectedPlayerSlots = new List<Texture2D> { playerOneSlot, emptyPlayerSlot, emptyPlayerSlot, emptyPlayerSlot };

            Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;
            // Set the textures on each menu element and its scale
            newGameMenuEntry.Texture = scrollTexture;
            newGameMenuEntry.TextureScale = scrollTextureScale;
            newGameMenuEntry.TextureAnimation = new Animation(activeScrollAnimation);

            playerSelectionMenuEntry.Texture = scrollTexture;
            playerSelectionMenuEntry.TextureScale = scrollTextureScale;
            playerSelectionMenuEntry.TextureAnimation = new Animation(activeScrollAnimation);

            // Now that they have textures, set the proper positions on the menu entries
            SetElementPosition(viewport);

            AudioController.PlaySong("background_music");
            base.LoadContent();
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
            float entryStart = titleHeight + marginY + textureHeight/2;

            for (int i = 0; i < MenuEntries.Count; i++)
            {
                float entrySpacing = (i == 0) ? 0.0f : textureHeight;

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
                        playerSelectionMenuEntry.Text = "Players connected";
                        bool isKeyboard = InputController.GetActiveInputs().Last().IsKeyboard;
                        Texture2D newPlayerSlotIcon = isKeyboard ? keyboardPlayerSlot : controllerPlayerSlot;
                        connectedPlayerSlots[activeInputs] = newPlayerSlotIcon;
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
            playerSelectionMenuEntry.Text = "Players connected";
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
                if (i == 2)
                {
                    DrawSpecialEntry(gameTime, menuEntry);
                }
                else
                {
                    bool isSelected = IsActive && (i == selectedEntry);
                    menuEntry.Draw(this, isSelected, gameTime);
                }
            }

            spriteBatch.End();
        }

        public void DrawSpecialEntry(GameTime gameTime, MenuEntry menuEntry)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            Texture2D texture = menuEntry.Texture;
            String text = menuEntry.Text;
            SpriteFont spriteFont = menuEntry.Font;
            Vector2 position = menuEntry.Position;
            float textureScale = menuEntry.TextureScale;

            float slotTextureWidth = emptyPlayerSlot.Width * slotScale;
            float slotTextureHeight = emptyPlayerSlot.Height * slotScale;
            float slotDisplayWidth = texture.Width * textureScale * 0.6f;
            float slotSpacing = (float)Math.Floor((slotDisplayWidth - 4 * slotTextureWidth) / 5);

            Vector2 textSize = spriteFont.MeasureString(text);
            float verticalSpacing = (float)Math.Floor((texture.Height * textureScale - textSize.Y - slotTextureHeight) / 3);

            Vector2 textPosition = position + new Vector2(
                (float)Math.Floor((texture.Width * textureScale - textSize.X) / 2),
                verticalSpacing);

            spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, textureScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(spriteFont, text, textPosition, Color.White);

            for(int i=0; i < connectedPlayerSlots.Count(); i++)
            {
                Vector2 slotPosition = position + new Vector2(
                (texture.Width * textureScale - slotDisplayWidth)/2 + slotSpacing * (i+1) + slotTextureWidth * i,
                textSize.Y + 2 * verticalSpacing);

                Color color = Color.White;

                if ((i + 1) <= gameStartDescription.NumberOfPlayers)
                {
                    color = playerColors[i];
                }
                
                spriteBatch.Draw(connectedPlayerSlots[i], slotPosition, null, color, 0f, Vector2.Zero, slotScale, SpriteEffects.None, 0f);
            }
            
        }

        #endregion
    }
}
