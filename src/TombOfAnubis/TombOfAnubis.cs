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

        public TombOfAnubis()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            screenManager = new GameScreenManager(this);
            Components.Add(screenManager);
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            ResolutionController.Initialize(graphics);

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

            base.Draw(gameTime);

        }
    }
}