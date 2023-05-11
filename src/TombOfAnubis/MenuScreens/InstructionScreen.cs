using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.MenuScreens
{
    class InstructionScreen : GameScreen
    {

        private List<Texture2D> instructionPages;
        private int currentPage;
        private bool invokedFromMain;

        private bool buttonPressed;
        private bool buttonCooldown;
        private TimeSpan lastPressed;


        public InstructionScreen(bool invokedFromMain) : base()
        {
            buttonPressed = true;
            currentPage = 0;
            this.invokedFromMain = invokedFromMain;
        }

        public override void LoadContent()
        {

            ContentManager content = GameScreenManager.Game.Content;
            Texture2D goalPage = content.Load<Texture2D>("Textures/Menu/InstructionScreen/Instructions1");
            Texture2D collabPage = content.Load<Texture2D>("Textures/Menu/InstructionScreen/Instructions2");
            Texture2D powerupPage = content.Load<Texture2D>("Textures/Menu/InstructionScreen/Instructions3");
            //Texture2D anubisPage = content.Load<Texture2D>("Textures/Menu/InstructionScreen/Instructions4");

            instructionPages = new List<Texture2D> { goalPage, collabPage, powerupPage };


            base.LoadContent();
        }

        public override void HandleInput()
        {
            if (!buttonCooldown)
            {
                if (InputController.IsUseTriggered())
                {
                    AudioController.PlaySoundEffect("menuAccept");
                    currentPage += 1;
                    buttonPressed = true;

                    if (currentPage == instructionPages.Count)
                    {
                        currentPage = instructionPages.Count - 1;
                        if (invokedFromMain) { GameScreenManager.RemoveScreen(this); }
                        else { GameScreenManager.RemoveScreen(this); }
                    }
                }
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                               bool coveredByOtherScreen)
        {
            // Update button cooldown.
            if (buttonPressed)
            {
                buttonPressed = false;
                buttonCooldown = true;
                lastPressed = gameTime.TotalGameTime;
            }
            if (buttonCooldown) // enough time elapsed since last pressed
            {
                TimeSpan diff = gameTime.TotalGameTime - lastPressed;
                if (diff.TotalMilliseconds > 250)
                {
                    buttonCooldown = false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;
            Viewport viewport = ResolutionController.TargetViewport;

            spriteBatch.Begin();

            Texture2D displayPage = instructionPages[currentPage];
            Rectangle displayPosition = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
            spriteBatch.Draw(displayPage, displayPosition, Color.White);

            spriteBatch.End();
        }
    }
}
