using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TombOfAnubis
{
    class Session
    {
        /// <summary>
        /// The single Session instance that can be active at a time.
        /// </summary>
        private static Session singleton;

        /// <summary>
        /// The GameplayScreen object that created this session.
        /// </summary>
        private GameplayScreen gameplayScreen;

        /// <summary>
        /// The ScreenManager used to manage all UI in the game.
        /// </summary>
        private GameScreenManager gameScreenManager;

        /// <summary>
        /// Returns true if there is an active session.
        /// </summary>
        public static bool IsActive
        {
            get { return singleton != null; }
        }


        private Map map;
        [ContentSerializerIgnore]
        public Map Map
        {
            get { return map; }
            set { map = value; }
        }


        /// <summary>
        /// The viewport that the tile engine is rendering within.
        /// </summary>
        private Viewport viewport;

        /// <summary>
        /// The viewport that the tile engine is rendering within.
        /// </summary>
        public Viewport Viewport
        {
            get { return viewport; }
            set
            {
                viewport = value;
                viewportCenter = new Vector2(
                     viewport.Width / 2f,
                     viewport.Height / 2f);
            }
        }

        /// <summary>
        /// The center of the current viewport.
        /// </summary>
        private Vector2 viewportCenter;
        public SpriteSystem SpriteSystem { get; set; }
        public CollisionSystem CollisionSystem { get; set; }
        public PlayerInputSystem PlayerInputSystem { get; set; }

        public Scene Scene { get; set; }



        // <summary>
        /// Private constructor of a Session object.
        /// </summary>
        /// <remarks>
        /// The lack of public constructors forces the singleton model.
        /// </remarks>
        private Session(GameScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameter
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // assign the parameter
            gameScreenManager = screenManager;
            this.gameplayScreen = gameplayScreen;
        }

        /// <summary>
        /// Update the session for this frame.
        /// </summary>
        /// <remarks>This should only be called if there are no menus in use.</remarks>
        public static void Update(GameTime gameTime)
        {
            // check the singleton
            if (singleton == null)
            {
                return;
            }
           singleton.CollisionSystem.Update(gameTime);
           singleton.PlayerInputSystem.Update(gameTime);

        }

        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime, int playerIndex)
        {
            singleton.SpriteSystem.Viewport = singleton.Viewport;
            singleton.SpriteSystem.Draw(gameTime);
        }

        /// <summary>
        /// Start a new session based on the data provided.
        /// </summary>
        public static void StartNewSession(GameStartDescription gameStartDescription,
            GameScreenManager screenManager, GameplayScreen gameplayScreen)
        {
            // check the parameters
            if (gameStartDescription == null)
            {
                throw new ArgumentNullException("gameStartDescripton");
            }
            if (screenManager == null)
            {
                throw new ArgumentNullException("screenManager");
            }
            if (gameplayScreen == null)
            {
                throw new ArgumentNullException("gameplayScreen");
            }

            // end any existing session
            EndSession();

            // create a new singleton
            singleton = new Session(screenManager, gameplayScreen);
            singleton.CollisionSystem = new CollisionSystem();
            singleton.SpriteSystem = new SpriteSystem(screenManager.SpriteBatch);
            singleton.PlayerInputSystem = new PlayerInputSystem();

            singleton.Scene = new Scene(Vector2.Zero);


            //// set up the initial map
            ChangeMap(gameStartDescription.MapContentName);

            for(int i = 0; i < gameStartDescription.NumberOfPlayers; i++)
            {
                Player player = new Player(i,
                    new Vector2(singleton.Map.SpawnMapPosition.X + 10 * i, singleton.Map.SpawnMapPosition.Y + 10 * i),
                    new Vector2(0.25f, 0.25f),
                    singleton.gameScreenManager.Game.Content.Load<Texture2D>("Textures/Characters/plagiarized_explorer"));
                singleton.Scene.AddChild(player);
            }
            List<Entity> mapEntities = singleton.Map.CreateMapEntities();
            singleton.Scene.AddChildren(mapEntities);

        }

        // <summary>
        /// End the current session.
        /// </summary>
        public static void EndSession()
        {
            // exit the gameplay screen
            // -- store the gameplay session, for re-entrance
            if (singleton != null)
            {
                GameplayScreen gameplayScreen = singleton.gameplayScreen;
                singleton.gameplayScreen = null;

                // clear the singleton
                singleton = null;

                if (gameplayScreen != null)
                {
                    gameplayScreen.ExitScreen();
                }
            }
        }

        /// <summary>
        /// Change the current map, arriving at the given portal if any.
        /// </summary>
        /// <param name="contentName">The asset name of the new map.</param>
        /// <param name="originalPortal">The portal from the previous map.</param>
        public static void ChangeMap(string contentName)
        {
            // make sure the content name is valid
            string mapContentName = contentName;
            if (!mapContentName.StartsWith(@"Maps\"))
            {
                mapContentName = Path.Combine(@"Maps", mapContentName);
            }

            // load the map
            ContentManager content = singleton.gameScreenManager.Game.Content;
            singleton.Map = content.Load<Map>(mapContentName);
        }

        public static void SetFocusOnPlayer(int playerIdx, Viewport viewport)
        {
            singleton.Viewport = viewport;

            foreach (PlayerInput playerInput in PlayerInputSystem.GetRegisteredComponents())
            {
                if(playerInput.PlayerID == playerIdx)
                {
                    Player player = playerInput.Entity as Player;
                    Transform playerTransform= player.GetComponent<Transform>();
                    Vector2 playerPosition = playerTransform.Position;
                    Vector2 playerScale = playerTransform.Scale;
                    Texture2D texture = player.GetComponent<Sprite>().Texture;
                    Vector2 playerCenter = new Vector2 (playerPosition.X + (texture.Width / 2) * playerScale.X, playerPosition.Y + (texture.Height / 2) * playerScale.Y);

                    singleton.Scene.GetComponent<Transform>().Position = singleton.viewportCenter - playerCenter;
                }
            }
        }

    }
}
