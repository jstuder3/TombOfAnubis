using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class GameScreenManager : DrawableGameComponent
    {
        List<GameScreen> _screens = new List<GameScreen>();
        List<GameScreen> _screensToUpdate = new List<GameScreen>();

        SpriteBatch spriteBatch;

        bool _traceEnabled;
        bool _isInitialized;


        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return _traceEnabled; }
            set { _traceEnabled = value; }
        }

        public GameScreenManager(Game game) : base(game)
        {

        }
        public override void Initialize()
        {
            base.Initialize();
            _isInitialized = true;
        }

        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Tell each of the _screens to load their content.
            foreach (GameScreen screen in _screens)
            {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the _screens to unload their content.
            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
        }



        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();

            foreach (GameScreen screen in _screens)
                _screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are _screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput();

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // _screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (_traceEnabled)
                TraceScreens();
        }


        /// <summary>
        /// Prints a list of all the _screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in _screens)
                screenNames.Add(screen.GetType().Name);

#if WINDOWS
            Trace.WriteLine(string.Join(", ", screenNames.ToArray()));
#endif
        }


        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen)
        {
            screen.GameScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (_isInitialized)
            {
                screen.LoadContent();
            }

            _screens.Add(screen);
        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);
        }


        /// <summary>
        /// Expose an array holding all the _screens. We return a copy rather
        /// than the real master list, because _screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }

    }
}
