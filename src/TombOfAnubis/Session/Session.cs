using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;


namespace TombOfAnubis
{
    public enum SessionState
    {
        Running,
        GameWon,
        GameOver
    }
    class Session
    {
        // TEMPORARY HARD CODED PARAMETERS

        public AnubisBehaviour AnubisBehaviour { get; set; } = AnubisBehaviour.Random;
        /// <summary>
        /// The single Session instance that can be active at a time.
        /// </summary>
        protected static Session singleton;

        public SessionState SessionState { get; set; }

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

        public static Session GetInstance() { return singleton; }


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
        public GameplayEffectSystem GameplayEffectSystem { get; set; }
        public InputSystem PlayerInputSystem { get; set; }
        public AISystem AnubisAISystem { get; set; }

        public DiscoverySystem DiscoverySystem { get; set; }

        public AnimationSystem AnimationSystem { get; set; }

        public MovementSystem MovementSystem { get; set; }

        public ButtonControllerSystem ButtonControllerSystem { get; set; }

        public Scene Scene { get; set; }

        public List<Texture2D> ArtefactTextures { get; set; }

        public int NumberOfPlayers { get; set; }

        public Entity[,] MapTiles { get; set; } 


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
            this.SessionState = SessionState.Running;
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
            singleton.PlayerInputSystem.Update(gameTime);
            singleton.CollisionSystem.Update(gameTime);
            singleton.MovementSystem.Update(gameTime);
            singleton.GameplayEffectSystem.Update(gameTime);
            singleton.AnimationSystem.Update(gameTime);
            singleton.DiscoverySystem.Update(gameTime);
            singleton.AnubisAISystem.Update(gameTime);
            singleton.ButtonControllerSystem.Update(gameTime);
        }

        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime)
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

            singleton.Scene = new Scene(Vector2.Zero);

            singleton.CollisionSystem = new CollisionSystem();
            singleton.SpriteSystem = new SpriteSystem(screenManager.SpriteBatch);
            singleton.PlayerInputSystem = new InputSystem(screenManager);
            singleton.GameplayEffectSystem = new GameplayEffectSystem();
            singleton.AnubisAISystem = new AISystem(singleton.Scene, AnubisBehaviour.Random);
            singleton.DiscoverySystem = new DiscoverySystem(singleton.Scene);
            singleton.AnimationSystem = new AnimationSystem();
            singleton.MovementSystem = new MovementSystem();
            singleton.ButtonControllerSystem = new ButtonControllerSystem();

            //// set up the initial map
            ChangeMap(gameStartDescription.MapContentName);

            singleton.NumberOfPlayers = gameStartDescription.NumberOfPlayers;

            for (int i = 0; i < gameStartDescription.NumberOfPlayers; i++)
            {
                EntityDescription character = singleton.Map.Characters[i];
                singleton.Scene.AddChild(new Character(
                    i,
                    singleton.Map.CreateEntityTileCenteredPosition(character),
                    character.Scale,
                    character.Texture,
                    singleton.Map.EntityProperties.MaxCharacterMovementSpeed,
                    character.Animation
                    ));

                EntityDescription artefact = singleton.Map.Artefacts[i];
                singleton.Scene.AddChild(new Artefact(
                    i,
                    singleton.Map.CreateEntityTileCenteredPosition(artefact),
                    artefact.Scale,
                    artefact.Texture,
                    true
                    ));
            }
            foreach( var dispenser in singleton.Map.Dispensers ) {
                _ = Enum.TryParse(dispenser.Type, out DispenserType type);
                singleton.Scene.AddChild(new Dispenser(
                    singleton.Map.CreateEntityTileCenteredPosition(dispenser),
                    dispenser.Scale,
                    dispenser.Texture,
                    type
                    ));
            }

            singleton.Scene.AddChild(new Anubis(
                                    singleton.Map.CreateEntityTileCenteredPosition(singleton.Map.Anubis),
                                    singleton.Map.Anubis.Scale,
                                    singleton.Map.Anubis.Texture,
                                    singleton.Map.Anubis.Animation,
                                    singleton.Map.EntityProperties.MaxAnubisMovementSpeed,
                                    singleton.Map));

            singleton.Scene.AddChild(new Altar(
                                    singleton.Map.CreateEntityTileCenteredPosition(singleton.Map.Altar),
                                    singleton.Map.Altar.Scale,
                                    singleton.Map.Altar.Texture,
                                    singleton.NumberOfPlayers));
            
            foreach(var trap in singleton.Map.Traps)
            {
                _ = Enum.TryParse(trap.Type, out TrapType type);
                singleton.Scene.AddChild(new Trap(
                    type,
                    singleton.Map.CreateEntityTileCenteredPosition(trap),
                    trap.Scale,
                    trap.Texture,
                    trap.Animation
                    ));
            }

            foreach(var button in singleton.Map.Buttons)
            {
                List<Vector2> connectedTraps = new List<Vector2>();

                foreach(EntityDescription trapEntity in button.ConnectedTrapPositions)
                {
                    connectedTraps.Add(singleton.Map.CreateEntityTileCenteredPositionSpriteless(trapEntity));
                }
                _ = Enum.TryParse(button.Type, out ButtonType type);
                singleton.Scene.AddChild(new Button(
                    type,
                    singleton.Map.CreateEntityTileCenteredPosition(button),
                    button.Scale,
                    button.Texture,
                    button.Animation,
                    connectedTraps
                    ));
            }

            List<Entity> mapEntities = singleton.CreateMapEntities();
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

                // clear systems
                SpriteSystem.Clear();
                CollisionSystem.Clear();
                GameplayEffectSystem.Clear();
                InputSystem.Clear();
                AISystem.Clear();
                DiscoverySystem.Clear();
                AnimationSystem.Clear();
                MovementSystem.Clear();
                ButtonControllerSystem.Clear();

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

            // Create the undiscovered texture. This should be loaded from a png and prettier than just a black square.
            singleton.Map.UndiscoveredTexture = new Texture2D(singleton.gameScreenManager.GraphicsDevice, 1, 1);
            singleton.Map.UndiscoveredTexture.SetData(new[] { Color.Black });

            singleton.DiscoverySystem.SetMap(singleton.Map);
        }

        public static void SetViewport(Viewport viewport)
        {
            singleton.Viewport = viewport;
        }

        public static void SetFocusOnPlayer(int playerIdx)
        {
            foreach (Input playerInput in InputSystem.GetRegisteredComponents())
            {
                if (playerInput.Entity.GetComponent<Player>().PlayerID == playerIdx)
                {
                    Character player = playerInput.Entity as Character;
                    Transform playerTransform = player.GetComponent<Transform>();
                    Vector2 playerPosition = playerTransform.Position;
                    Vector2 playerSize = player.Size();
                    Vector2 playerCenter = new Vector2(playerPosition.X + playerSize.X / 2, playerPosition.Y + playerSize.Y / 2);

                    singleton.Scene.GetComponent<Transform>().Position = singleton.viewportCenter - playerCenter;
                }
            }
        }
        public static void MoveMapCenterTo(Vector2 newMapCenter)
        {
            Vector2 mapCenter = new Vector2(
                singleton.Map.MapDimensions.X * singleton.Map.TileSize.X * singleton.Scene.GetComponent<Transform>().Scale.X / 2,
                singleton.Map.MapDimensions.Y * singleton.Map.TileSize.Y * singleton.Scene.GetComponent<Transform>().Scale.Y / 2);
            singleton.Scene.GetComponent<Transform>().Position = newMapCenter - mapCenter;
        }
        public List<Entity> CreateMapEntities()
        {
            List<Entity> entities = new List<Entity>();
            singleton.MapTiles = new Entity[singleton.Map.MapDimensions.X, singleton.Map.MapDimensions.Y];

            for (int y = 0; y < singleton.Map.MapDimensions.Y; y++)
            {
                for (int x = 0; x < singleton.Map.MapDimensions.X; x++)
                {

                    Point mapPosition = new Point(x, y);
                    bool wall = singleton.Map.GetCollisionLayerValue(mapPosition) == 1;
                    Rectangle sourceRectangle = singleton.Map.GetBaseLayerSourceRectangle(mapPosition);
                    Vector2 position = new Vector2(x * singleton.Map.TileSize.X, y * singleton.Map.TileSize.Y);

                    if (sourceRectangle != Rectangle.Empty)
                    {
                        if (wall)
                        {
                            Wall newWall = new Wall(position, singleton.Map.TileScale, singleton.Map.Texture, singleton.Map.UndiscoveredTexture, sourceRectangle);
                            entities.Add(newWall);
                            singleton.MapTiles[x, y] = newWall;

                        }
                        else
                        {
                            Floor newFloor = new Floor(position,  singleton.Map.TileScale, singleton.Map.Texture, singleton.Map.UndiscoveredTexture, sourceRectangle);
                            entities.Add(newFloor);
                            singleton.MapTiles[x, y] = newFloor;

                        }
                    }
                }
            }
            return entities;
        }

    }
}
