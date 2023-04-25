using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TombOfAnubis
{
    public class IntroScreen : GameScreen
    {
        private SpriteBatch spriteBatch;

        private SpriteFont statusFont = Fonts.DisneyHeroicFont;
        private Color statusColor = Color.Gold;
        private float fontScale = 1f;

        public IntroScreen()
            : base()
        {
            InputController.ResetPlayerInputs();
        }
        public override void LoadContent()
        {
            spriteBatch = GameScreenManager.SpriteBatch;
            VideoController.PlayVideo(@"Content/Videos/IntroVideo_v2.mp4", false, false);
        }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }
        public override void HandleInput()
        {
            foreach (PlayerInput playerInput in InputController.PlayerInputs)
            {
                if (playerInput.UseTriggered())
                {

                    InputController.AddCooldown(playerInput.UseKey, playerInput.UseButton, 250);
                    playerInput.IsActive = true;
                    playerInput.PlayerID = 0;
                    VideoController.StopVideo();
                    ExitScreen();
                    GameScreenManager.AddScreen(new MainMenuScreen());
                }
            }
            if(Keyboard.GetState().IsKeyDown(Keys.P))
            {
                ExitScreen();
                GameStartDescription gameStartDescription = new GameStartDescription();
                gameStartDescription.MapContentName = "Map001";
                gameStartDescription.NumberOfPlayers = 4;
                int activeInputs = 0;
                foreach(PlayerInput input in InputController.PlayerInputs)
                {
                    input.IsActive = true;
                    input.PlayerID = activeInputs;
                    activeInputs++;
                    if(activeInputs == 4) { break; }
                }
                LoadingScreen.Load(GameScreenManager, true, new GameplayScreen(gameStartDescription));
            }
        }
        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;
            //var videoTexture = videoPlayer.GetTexture();
            VideoController.Draw(spriteBatch, new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height));

            string statusText = "Press [E] / (A)";
            Vector2 textLength = statusFont.MeasureString(statusText) * fontScale;

            Vector2 displayPosition = new Vector2(viewport.X + viewport.Width * 4f/5f, viewport.Y + viewport.Height * 4f/5f);
            spriteBatch.Begin();
            spriteBatch.DrawString(statusFont, statusText, displayPosition, statusColor,
            0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
