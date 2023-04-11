using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class IntroScreen : GameScreen
    {
        private SpriteBatch spriteBatch;

        private SpriteFont statusFont = Fonts.DisneyHeroicFont;
        private Color statusColor = Color.Red;
        private float fontScale = 1f;

        public IntroScreen()
            : base()
        {
            
        }
        public override void LoadContent()
        {
            spriteBatch = GameScreenManager.SpriteBatch;
            VideoController.PlayVideo("test_video", true);
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
