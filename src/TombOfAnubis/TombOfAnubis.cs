using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class TombOfAnubis : Game
    {
        private static GraphicsDeviceManager graphics;
        GameScreenManager screenManager;

        public static int numSupportedResolutions = 2;
        public static ushort[] supportedWidths = new ushort[] { 2560, 1920 };
        public static ushort[] supportedHeights = new ushort[] { 1440, 1080 };

        public static void ToggleFullScreen()
        {
            graphics.ToggleFullScreen();
        }

        public static void ChangeResolution(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            // Apply the changes
            graphics.ApplyChanges();
        }


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