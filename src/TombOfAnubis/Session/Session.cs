using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    class Session
    {
        /// <summary>
        /// The single Session instance that can be active at a time.
        /// </summary>
        private static Session singleton;

        /// <summary>
        /// The GameplayScreen object that created this session.
        /// </summary>
        private GameplayScreen gameplayScreen;

        /// <summary>
        /// The ScreenManager used to manage all UI in the game.
        /// </summary>
        private GameScreenManager gameScreenManager;

        /// <summary>
        /// Returns true if there is an active session.
        /// </summary>
        public static bool IsActive
        {
            get { return singleton != null; }
        }

        // <summary>
        /// Private constructor of a Session object.
        /// </summary>
        /// <remarks>
        /// The lack of public constructors forces the singleton model.
        /// </remarks>
        private Session(GameScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameter
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // assign the parameter
            gameScreenManager = screenManager;
            this.gameplayScreen = gameplayScreen;
        }

        /// <summary>
        /// Update the session for this frame.
        /// </summary>
        /// <remarks>This should only be called if there are no menus in use.</remarks>
        public static void Update(GameTime gameTime)
        {
            // check the singleton
            if (singleton == null)
            {
                return;
            }

            //if (CombatEngine.IsActive)
            //{
            //    CombatEngine.Update(gameTime);
            //}
            //else
            //{
            //    singleton.UpdateQuest();
            //    TileEngine.Update(gameTime);
            //}
        }

        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = singleton.gameScreenManager.SpriteBatch;

            //if (CombatEngine.IsActive)
            //{
            //    // draw the combat background
            //    if (TileEngine.Map.CombatTexture != null)
            //    {
            //        spriteBatch.Begin();
            //        spriteBatch.Draw(TileEngine.Map.CombatTexture, Vector2.Zero,
            //            Color.White);
            //        spriteBatch.End();
            //    }

            //    // draw the combat itself
            //    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            //    CombatEngine.Draw(gameTime);
            //    spriteBatch.End();
            //}
            //else
            //{
            //    singleton.DrawNonCombat(gameTime);
            //}

            //singleton.hud.Draw();
        }

        /// <summary>
        /// Start a new session based on the data provided.
        /// </summary>
        public static void StartNewSession(GameStartDescription gameStartDescription,
            GameScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameters
            if (gameStartDescription == null)
            {
                throw new ArgumentNullException("gameStartDescripton");
            }
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // end any existing session
            EndSession();

            // create a new singleton
            singleton = new Session(screenManager, gameplayScreen);

            //// set up the initial map
            //ChangeMap(gameStartDescription.MapContentName, null);

            // set up the initial party
            //ContentManager content = singleton.screenManager.Game.Content;
            //singleton.party = new Party(gameStartDescription, content);

        }

        // <summary>
        /// End the current session.
        /// </summary>
        public static void EndSession()
        {
            // exit the gameplay screen
            // -- store the gameplay session, for re-entrance
            if (singleton != null)
            {
                GameplayScreen gameplayScreen = singleton.gameplayScreen;
                singleton.gameplayScreen = null;

                //// pop the music
                //AudioManager.PopMusic();

                // clear the singleton
                singleton = null;

                if (gameplayScreen != null)
                {
                    gameplayScreen.ExitScreen();
                }
            }
        }

    }
}
