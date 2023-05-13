using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Framework.Media;
using MonoGame.Extended.VideoPlayback;
using TombOfAnubis.MenuScreens;

namespace TombOfAnubis
{
    class PauseMenuScreen : MenuScreen
    {
        private GameStartDescription gameStartDescription;

        #region Graphics Data


        private Texture2D backgroundTexture;
        private Rectangle backgroundPosition;

        private Vector2 titlePosition;
        private Texture2D titleTexture;
        private float titleScale = 0.5f;

        private Texture2D scrollTexture;
        private float scrollTextureScale = 0.4f;
        private int scrollTextureWidth = 800, scrollTextureHeight = 400;

        private float marginX = 0.25f, marginY = 0.04f;

        #endregion


        #region Menu Entries


        MenuEntry resumeMenuEntry, restartMenuEntry, instructionsMenuEntry, endGameMenuEntry;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public PauseMenuScreen() : base()
        {
            gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map001";
            gameStartDescription.NumberOfPlayers = InputController.GetActiveInputs().Count;

            // Add the Resume entry
            resumeMenuEntry = new MenuEntry("Resume");
            resumeMenuEntry.Font = Fonts.DisneyHeroicFont;
            resumeMenuEntry.Selected += ResumeMenuEntrySelected;
            MenuEntries.Add(resumeMenuEntry);

            // Add the New Game entry
            restartMenuEntry = new MenuEntry("Restart");
            restartMenuEntry.Font = Fonts.DisneyHeroicFont;
            restartMenuEntry.Selected += RestartMenuEntrySelected;
            MenuEntries.Add(restartMenuEntry);

            // Create the  Instructions entry
            instructionsMenuEntry = new MenuEntry("Instructions");
            instructionsMenuEntry.Font = Fonts.DisneyHeroicFont;
            instructionsMenuEntry.Selected += InstructionsMenuEntrySelected;
            MenuEntries.Add(instructionsMenuEntry);

            // Create the Exit menu entry
            endGameMenuEntry = new MenuEntry("Main Menu");
            endGameMenuEntry.Font = Fonts.DisneyHeroicFont;
            endGameMenuEntry.Selected += EndGameMenuEntrySelected;
            MenuEntries.Add(endGameMenuEntry);
        }


        /// <summary>
        /// Load the graphics content for this screen.
        /// </summary>
        public override void LoadContent()
        {

            AudioController.PauseSong();

            // Load the textures
            ContentManager content = GameScreenManager.Game.Content;
            backgroundTexture = content.Load<Texture2D>("Textures/Menu/PauseScreenBG");
            scrollTexture = content.Load<Texture2D>("Textures/Menu/Scroll");
            titleTexture = content.Load<Texture2D>("Textures/Menu/Title_advanced");

            foreach(MenuEntry entry in MenuEntries)
            {
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

            // Center background image around viewport
            backgroundPosition = new Rectangle(viewport.X,viewport.Y, screenWidth, screenHeight);

            float titleWidth = titleScale * titleTexture.Width / screenWidth;
            float titleHeight = titleScale * titleTexture.Height / screenHeight;
            // Assume every entry has the same sized texture
            float textureWidth = ((float)scrollTextureWidth / screenWidth) * scrollTextureScale;
            float textureHeight = ((float)scrollTextureHeight / screenHeight) * scrollTextureScale;

            // Center the title according to the screen width
            //float titleOffsetX = (1.0f - titleWidth) / 2;
            float titleOffsetX = marginX + (textureWidth - titleWidth) / 2;
            titlePosition = GetRelativePosition(viewport, titleOffsetX, marginY);

            // Center the UI element according to the screen width
            float textureOffsetX = (1.0f - textureWidth) / 2;
            // The first MenuEntry element is drawn at this relative vertical coordinate
            float entryStart = titleHeight + 2.5f * marginY;

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
        /// Event handler for when the Restart menu entry is selected.
        /// </summary>
        void ResumeMenuEntrySelected(object sender, EventArgs e)
        {
            AudioController.ResumeSong();
            ExitScreen();
        }

        /// <summary>
        /// Event handler for when the Restart menu entry is selected.
        /// </summary>
        void RestartMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                ExitScreen();
            }
            Session.EndSession();
            LoadingScreen.LoadAtRestart(GameScreenManager, true, new GameplayScreen(gameStartDescription));
        }

        /// <summary>
        /// Event handler for when the Controls menu entry is selected.
        /// </summary>
        void InstructionsMenuEntrySelected(object sender, EventArgs e)
        {
            GameScreenManager.AddScreen(new InstructionScreen(false));
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