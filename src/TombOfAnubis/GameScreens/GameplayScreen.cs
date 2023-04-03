using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TombOfAnubis
{
    class GameplayScreen : GameScreen
    {

        GameStartDescription gameStartDescription = null;

        public Hud Hud {  get; set; }

        private SpriteBatch SpriteBatch;

        /// <summary>
        /// Create a new GameplayScreen object.
        /// </summary>
        public GameplayScreen()
            : base()
        {
            Exiting += new EventHandler(GameplayScreen_Exiting);
        }

        public GameplayScreen(GameStartDescription gameStartDescription)
            : this()
        {
            this.gameStartDescription = gameStartDescription;
        }

        /// <summary>
        /// Handle the closing of this screen.
        /// </summary>
        void GameplayScreen_Exiting(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            // TODO: Will be removed and called directly from Menu

            if (this.gameStartDescription == null)
            {
                this.gameStartDescription = new GameStartDescription();
                this.gameStartDescription.MapContentName = "Map001";
                this.gameStartDescription.NumberOfPlayers = 4;

            }
            SpriteBatch = GameScreenManager.SpriteBatch;
            SplitScreen.Initialize(GameScreenManager.GraphicsDevice, gameStartDescription.NumberOfPlayers);
            Session.StartNewSession(gameStartDescription, GameScreenManager, this);
            Hud = new Hud(GameScreenManager.GraphicsDevice);


            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            GameScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive && !coveredByOtherScreen)
            {
                Session.Update(gameTime);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput()
        {
            //if (InputManager.IsActionTriggered(InputManager.Action.MainMenu))
            //{
            //    GameScreenManager.AddScreen(new MainMenuScreen());
            //    return;
            //}

            //if (InputManager.IsActionTriggered(InputManager.Action.ExitGame))
            //{
            //    // add a confirmation message box
            //    const string message =
            //        "Are you sure you want to exit?  All unsaved progress will be lost.";
            //    MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
            //    confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            //    GameScreenManager.AddScreen(confirmExitMessageBox);
            //    return;
            //}

            //if (!CombatEngine.IsActive &&
            //    InputManager.IsActionTriggered(InputManager.Action.CharacterManagement))
            //{
            //    GameScreenManager.AddScreen(new StatisticsScreen(Session.Party.Players[0]));
            //    return;
            //}
        }


        /// <summary>
        /// Event handler for when the user selects Yes 
        /// on the "Are you sure?" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            GameScreenManager.Game.Exit();
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            for (int playerIdx = 0; playerIdx < SplitScreen.NumberOfPlayers; playerIdx++)
            {
                Viewport viewport = SplitScreen.SetViewport(playerIdx);

                Session.SetViewport(viewport);
                Session.SetFocusOnPlayer(playerIdx);
                SpriteBatch.Begin();
                Session.Draw(gameTime);
                SpriteBatch.End();
            }
            Viewport defaultViewport = SplitScreen.ResetViewport();
            Session.SetViewport(defaultViewport);
            SpriteBatch.Begin();
            Hud.Draw(gameTime);
            SpriteBatch.End();
        }
    }
}
