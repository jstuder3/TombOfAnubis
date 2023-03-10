using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    class GameplayScreen : GameScreen
    {


        /// <summary>
        /// Create a new GameplayScreen object.
        /// </summary>
        public GameplayScreen()
            : base()
        {
            Exiting += new EventHandler(GameplayScreen_Exiting);
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
                //Session.Update(gameTime);
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
            //Session.Draw(gameTime);
            var ii = 22;
        }
    }
}
