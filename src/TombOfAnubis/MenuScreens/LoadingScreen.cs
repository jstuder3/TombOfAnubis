#region File Description
//-----------------------------------------------------------------------------
// LoadingScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System; using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace TombOfAnubis
{
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    /// <remarks>
    /// Similar to a class found in the Game State Management sample on the 
    /// XNA Creators Club Online website (http://creators.xna.com).
    /// </remarks>
    class LoadingScreen : GameScreen
    {
        #region Screens Data

        bool loadingIsSlow;
        bool otherScreensAreGone;

        GameScreen[] screensToLoad;


        #endregion


        #region Graphics Data


        private Texture2D loadingTexture;
        private Rectangle loadingPosition;

        private string readyText = "Press <Use> to Start";
        private int useButtonCooldown = 250;
        private bool startGame = false;

        private SpriteFont font;

        private int numPlayers;
        private bool hasVideo;
        private float fontScale = 0.6f;
        // Relative to Viewport height
        private float margin = 0.08f;

        /// <summary>
        /// Constants to display skip option after some time
        /// </summary>

        // In milliseconds
        private int cooldownPeriod = 3000;
        private int afterSkipCooldown = 50;

        private bool startTimeSet = false;
        private bool skipCooldown = false;
        private double videoStartTime;


        #endregion


        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private LoadingScreen(GameScreenManager screenManager, bool loadingIsSlow,
                              GameScreen[] screensToLoad, bool hasVideo)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);

            this.hasVideo = hasVideo;
        }


        /// <summary>
        /// Activates the loading screen. (Background story video)
        /// </summary>
        public static void Load(GameScreenManager screenManager, bool loadingIsSlow,
                                params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad,
                                                            true);

            screenManager.AddScreen(loadingScreen);
        }

        /// <summary>
        /// Activates the loading screen. (Only show image)
        /// </summary>
        public static void LoadAtRestart(GameScreenManager screenManager, bool loadingIsSlow,
                                params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad,
                                                            false);

            screenManager.AddScreen(loadingScreen);
        }


        public override void LoadContent()
        {
            ContentManager content = GameScreenManager.Game.Content;
            Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;
            numPlayers = InputController.GetActiveInputs().Count;
            font = Fonts.SettingsTitleFont;

            if (hasVideo)
            {
                VideoController.LoadBackstoryVideo(numPlayers);
                loadingPosition = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);

                AudioController.PlaySong("gameWonTrack");

                switch (numPlayers)
                {
                    case 1: VideoController.PlayVideo(@"Content/Videos/Backstory1.mp4", false, true); break;

                    case 2: VideoController.PlayVideo(@"Content/Videos/Backstory2.mp4", false, true); break;

                    case 3: VideoController.PlayVideo(@"Content/Videos/Backstory3.mp4", false, true); break;

                    case 4: VideoController.PlayVideo(@"Content/Videos/Backstory4.mp4", false, true); break;
                }
            }

            loadingPosition = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);

            switch (numPlayers)
            {
                case 1: loadingTexture = content.Load<Texture2D>("Textures/Menu/LoadingScreen/BackstoryEnd1"); break;

                case 2: loadingTexture = content.Load<Texture2D>("Textures/Menu/LoadingScreen/BackstoryEnd2"); break;

                case 3: loadingTexture = content.Load<Texture2D>("Textures/Menu/LoadingScreen/BackstoryEnd3"); break;

                case 4: loadingTexture = content.Load<Texture2D>("Textures/Menu/LoadingScreen/BackstoryEnd4"); break;
            }

            base.LoadContent();
        }

        #endregion

        #region Handle Input

        public override void HandleInput()
        {
            if(InputController.IsUseTriggered() && useButtonCooldown <= 0)
            {
                useButtonCooldown = 250;
                if (skipCooldown || !hasVideo)
                {
                    AudioController.StopSong();
                    AudioController.PlaySoundEffect("menuAccept");
                    VideoController.StopVideo();

                    // To draw explaination picture after skipping video
                    hasVideo = false;
                    startGame = true;

                }
            }
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            if (hasVideo)
            {
                if (!startTimeSet)
                {
                    startTimeSet = true;
                    videoStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                }
                double diff = gameTime.TotalGameTime.TotalMilliseconds - videoStartTime;
                if (diff > cooldownPeriod)
                {
                    skipCooldown = true;
                }
            }


            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (useButtonCooldown > 0)
            {
                useButtonCooldown -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (otherScreensAreGone && startGame)
            {
                afterSkipCooldown -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                Draw(gameTime);

                if (afterSkipCooldown <= 0)
                {
                    GameScreenManager.RemoveScreen(this);

                    foreach (GameScreen screen in screensToLoad)
                    {
                        if (screen != null)
                        {
                            GameScreenManager.AddScreen(screen);
                        }
                    }

                    //// Once the load has finished, we use ResetElapsedTime to tell
                    //// the  game timing mechanism that we have just finished a very
                    //// long frame, and that it should not try to catch up.
                    GameScreenManager.Game.ResetElapsedTime();
                }
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (GameScreenManager.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (loadingIsSlow)
            {
                SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

                // Center the text in the viewport.
                Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;

                if(hasVideo)
                {
                    VideoController.Draw(spriteBatch, loadingPosition);

                    if (otherScreensAreGone && skipCooldown)
                    {
                        Vector2 textDimension = font.MeasureString(readyText) * fontScale;
                        Vector2 textPosition = new Vector2(viewport.Width - margin * viewport.Height - textDimension.X, (1 - margin) * viewport.Height - textDimension.Y);

                        spriteBatch.Begin();
                        spriteBatch.DrawString(font, readyText, textPosition, Color.Red, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
                        spriteBatch.End();
                    }
                }

                else
                {
                    Vector2 textDimension = font.MeasureString(readyText) * fontScale;
                    Vector2 textPosition = new Vector2(viewport.Width - margin * viewport.Height - textDimension.X, (1 - margin) * viewport.Height - textDimension.Y);

                    spriteBatch.Begin();
                    spriteBatch.Draw(loadingTexture, loadingPosition, Color.White);
                    spriteBatch.DrawString(font, readyText, textPosition, Color.Red, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
                    spriteBatch.End();
                }

            }
        }


        #endregion
    }
}
