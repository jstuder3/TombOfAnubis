using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TombOfAnubis
{
    public class TombOfAnubis : Game
    {
        private GraphicsDeviceManager graphics;
        GameScreenManager screenManager;

        SpriteBatch spriteBatch;

        Character player_1;

        public TombOfAnubis()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            screenManager = new GameScreenManager(this);
            Components.Add(screenManager);

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            TileEngine.Viewport = graphics.GraphicsDevice.Viewport;

            screenManager.AddScreen(new GameplayScreen());

        }

        protected override void LoadContent()
        {

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Transparent);

            base.Draw(gameTime);
        }
    }
}