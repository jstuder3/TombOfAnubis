using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;

namespace TombOfAnubis
{
    public class GameWonScreen : GameScreen
    {
        private SpriteBatch spriteBatch;

        private SpriteFont statusFont = Fonts.DisneyHeroicFont;
        private Color statusColor = Color.Gold;
        private float fontScale = 1f;

        // In milliseconds
        private int cooldownPeriod = 6750;
        private double videoStartTime;
        private bool skipCooldown = false;
        private bool startTimeSet = false;

        private List<PlayerInput> activeInputs;

        public GameWonScreen()
            : base()
        {
            activeInputs = InputController.GetActiveInputs();
        }
        public override void LoadContent()
        {
            AudioController.StopSong();
            AudioController.PlaySong("gameWonTrack");
            spriteBatch = GameScreenManager.SpriteBatch;

            switch (activeInputs.Count)
            {
                case 1: VideoController.PlayVideo(@"Content/Videos/GameWon1.mp4", false, true); break;

                case 2: VideoController.PlayVideo(@"Content/Videos/GameWon2.mp4", false, true); break;

                case 3: VideoController.PlayVideo(@"Content/Videos/GameWon3.mp4", false, true); break;

                case 4: VideoController.PlayVideo(@"Content/Videos/GameWon4.mp4", false, true); break;

            }
            
        }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            if (!startTimeSet)
            {
                startTimeSet = true;
                videoStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            }
            double diff = gameTime.TotalGameTime.TotalMilliseconds - videoStartTime;
            if (diff >  cooldownPeriod)
            {
                skipCooldown = true;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }
        public override void HandleInput()
        {
            foreach (PlayerInput playerInput in activeInputs)
            {
                if (playerInput.UseTriggered() && skipCooldown)
                {

                    InputController.AddCooldown(playerInput.UseKey, playerInput.UseButton, 250);
                    VideoController.StopVideo();
                    AudioController.StopSong();
                    RemoveSecondaryInputs();
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
            
            if (skipCooldown)
            {
                string statusText = "Press [E] / (A)";
                Vector2 textLength = statusFont.MeasureString(statusText) * fontScale;

                Vector2 displayPosition = new Vector2(viewport.X + viewport.Width * 4f / 5f, viewport.Y + viewport.Height * 4f / 5f);
                spriteBatch.Begin();
                spriteBatch.DrawString(statusFont, statusText, displayPosition, statusColor,
                0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
                spriteBatch.End();
            }

        }
    }
}
