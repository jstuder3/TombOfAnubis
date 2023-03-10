using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tomb_of_Anubis
{
    public class TombOfAnubis : Game
    {
        private GraphicsDeviceManager _graphics;
        GameScreenManager _screenManager;

        public TombOfAnubis()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //_graphics.PreferredBackBufferWidth = 1280;
            //_graphics.PreferredBackBufferHeight = 720;

            _screenManager = new GameScreenManager(this);
            Components.Add(_screenManager);

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            _screenManager.AddScreen(new GameplayScreen());

        }

        protected override void LoadContent()
        {
            //_spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Transparent);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}