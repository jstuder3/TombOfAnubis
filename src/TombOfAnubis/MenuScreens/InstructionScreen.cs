using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private Texture2D nextButton;
        private float marginRight = 0.06f, marginBottom = 0.09f;
        private float minButtonScale = 0.4f, maxButtonScale = 0.5f;
        private float currentScale = 0.4f, scaleStep = 0.001f;
        private bool isGrowing = true;


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
            Texture2D anubisPage = content.Load<Texture2D>("Textures/Menu/InstructionScreen/Instructions4");

            instructionPages = new List<Texture2D> { goalPage, collabPage, powerupPage, anubisPage };

            PlayerInput firstPlayer = InputController.GetActiveInputs()[0];

            switch (firstPlayer.UseKey)
            {
                default: nextButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/UseController"); break;

                case Keys.E: nextButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/Use1"); break;
                case Keys.Z: nextButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/Use2"); break;
                case Keys.O: nextButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/Use3"); break;
                case Keys.OemMinus: nextButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/Use4"); break;
            }


            base.LoadContent();
        }

        public override void HandleInput()
        {
            if (InputController.IsBackTriggered() && !Session.IsActive)
            {
                GameScreenManager.AddScreen(new MainMenuScreen());
                ExitScreen();
            }
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

            marginRight = 0.05f; marginBottom = 0.07f; maxButtonScale = 0.45f;

            spriteBatch.Begin();

            Texture2D displayPage = instructionPages[currentPage];
            Rectangle displayPosition = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
            spriteBatch.Draw(displayPage, displayPosition, Color.White);


            currentScale = isGrowing ? (currentScale + scaleStep) : (currentScale - scaleStep);
            if (isGrowing) isGrowing = (currentScale < maxButtonScale);
            else { isGrowing = (currentScale < minButtonScale);  }

            int buttonWidth = (int)(nextButton.Width * currentScale);
            int buttonHeight = (int)(nextButton.Height * currentScale);
            int offsetX = (int)((1 - marginRight) * viewport.Width);
            int offsetY = (int)((1 - marginBottom) * viewport.Height);

            Rectangle nextButtonPos = new Rectangle(offsetX, offsetY, buttonWidth, buttonHeight);
            Vector2 origin = new Vector2(nextButton.Width / 2, nextButton.Height / 2);
            spriteBatch.Draw(nextButton, nextButtonPos, null, Color.White, 0f, origin, SpriteEffects.None, 0f);


            spriteBatch.End();
        }
    }
}
