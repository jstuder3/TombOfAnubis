using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    class GameOverScreen : MenuScreen
    {
        private GameStartDescription gameStartDescription;

        #region Graphics Data


        private Texture2D backgroundTexture;
        private Vector2 backgroundPosition;

        private Vector2 titlePosition;
        private Texture2D titleTexture;
        private float titleScale = 0.4f;

        private Texture2D scrollTexture;
        private float scrollTextureScale = 0.4f;
        private static List<AnimationClip> activeScrollAnimation;
        private int scrollTextureWidth = 800, scrollTextureHeight = 400;

        private float marginY = 0.05f;

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
            gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map001";
            gameStartDescription.NumberOfPlayers = InputController.GetActiveInputs().Count;

            // Add the New Game entry
            restartMenuEntry = new MenuEntry("Restart");
            restartMenuEntry.Font = Fonts.DisneyHeroicFont;
            restartMenuEntry.Selected += RestartMenuEntrySelected;
            MenuEntries.Add(restartMenuEntry);

            // Create the Exit menu entry
            endGameMenuEntry = new MenuEntry("End Game");
            endGameMenuEntry.Font = Fonts.DisneyHeroicFont;
            endGameMenuEntry.Selected += EndGameMenuEntrySelected;
            MenuEntries.Add(endGameMenuEntry);
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

            // AudioController.PlaySong("background_music");
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
            float textureWidth = ((float)scrollTextureWidth / screenWidth) * scrollTextureScale;
            float textureHeight = ((float)scrollTextureHeight / screenHeight) * scrollTextureScale;

            // Center the title according to the screen width
            float titleOffsetX = (1.0f - titleWidth) / 2;
            titlePosition = GetRelativePosition(viewport, titleOffsetX, marginY);

            // Center the UI element according to the screen width
            float textureOffsetX = (1.0f - textureWidth) / 2;
            // The first MenuEntry element is drawn at this relative vertical coordinate
            float entryStart = titleHeight + 3 * marginY;

            for (int i = 0; i < MenuEntries.Count; i++)
            {
                float entrySpacing = i * textureHeight;

                float offsetY = entryStart + entrySpacing;

                MenuEntries[i].Position = GetRelativePosition(viewport, textureOffsetX, offsetY);
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
        /// Event handler for when the Restart menu entry is selected.
        /// </summary>
        void RestartMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }
            LoadingScreen.Load(GameScreenManager, true, new GameplayScreen(gameStartDescription));
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
            LoadingScreen.Load(GameScreenManager, true, new IntroScreen());
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
