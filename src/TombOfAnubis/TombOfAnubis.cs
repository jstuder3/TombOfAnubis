using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TombOfAnubis.PlayerCharacter;

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
            //_graphics.PreferredBackBufferWidth = 1280;
            //_graphics.PreferredBackBufferHeight = 720;

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
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            InputController inputController = new InputController();

            Texture2D plagiarized_explorer = Content.Load<Texture2D>("Textures/Characters/plagiarized_explorer");
            player_1 = new Character(PlayerType.Player, 100, 100, 0, 0, plagiarized_explorer, 1, inputController);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            player_1.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();

            // TODO: Add your drawing code here
            player_1.Draw(gameTime, spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}