using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace TombOfAnubis
{
    public class TombOfAnubis : Game
    {
        private GraphicsDeviceManager graphics;
        GameScreenManager screenManager;

        public static ScreenResizer resizer;

        public TombOfAnubis()
        {
            int width = 1920, height = 1080;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            screenManager = new GameScreenManager(this);
            Components.Add(screenManager);
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            screenManager.AddScreen(new IntroScreen(0));

        }

        protected override void LoadContent()
        {
            resizer = new ScreenResizer(graphics, base.Window, 1920, 1080);
            Fonts.LoadContent(Content);
            AudioController.LoadContent(Content);
            VideoController.LoadContent(graphics.GraphicsDevice);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            //Debug.WriteLine("(ToA) Viewport: " + graphics.GraphicsDevice.Viewport);
            InputController.Update(gameTime);
            VideoController.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //graphics.GraphicsDevice.Clear(Color.Transparent);
            graphics.GraphicsDevice.SetRenderTarget(resizer.renderTarget);
            graphics.GraphicsDevice.Clear(resizer.renderTargetColor);

            base.Draw(gameTime);

            graphics.GraphicsDevice.SetRenderTarget(null);
            graphics.GraphicsDevice.Clear(resizer.renderTargetColor);

            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(resizer.renderTarget, resizer.renderTargetDestination, Color.White);
            spriteBatch.End();
        }
    }
}