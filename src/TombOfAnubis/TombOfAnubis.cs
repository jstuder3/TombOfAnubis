using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class TombOfAnubis : Game
    {
        private GraphicsDeviceManager graphics;
        GameScreenManager screenManager;


        public TombOfAnubis()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            //graphics.IsFullScreen = true;

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
            Fonts.LoadContent(Content);
            AudioController.LoadContent(Content);
            VideoController.LoadContent(graphics.GraphicsDevice);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            InputController.Update(gameTime);
            VideoController.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Transparent);

            base.Draw(gameTime);
        }
    }
}