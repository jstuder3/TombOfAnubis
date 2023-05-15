using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using static System.Collections.Specialized.BitVector32;
using System.Linq;
using TombOfAnubis.GameScreens;

namespace TombOfAnubis
{
    public enum SessionState
    {
        Running,
        GameWon,
        GameOver
    }
    public enum Visibility
    {
        Both,
        Game,
        Minimap
    }
    class Session
    {
        // TEMPORARY HARD CODED PARAMETERS

        // END TEMPORARY HARD CODED PARAMETERS

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

        public bool IsEarthquake { get; set; } = false;

        private float shakeCooldown;
        private bool shakedLeft;
        private Vector2 earthQuakeOffset;
        public bool PauseDrawing { get; set; } = false;

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

        public Visibility Visibility { get; set; }

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

        public ParticleEmitterSystem ParticleEmitterSystem { get; set; }

        public WorldEventSystem WorldEventSystem { get; set; }

        public World World { get; set; }

        public List<Texture2D> ArtefactTextures { get; set; }
        public List<Texture2D> CharacterTextures { get; set; }
        public List<Texture2D> GhostCharacterTextures { get; set; }

        public Texture2D WorldEffectTexture { get; set; }

        public AnimationClip WorldEffectAnimation { get; set; }

        public Texture2D MinimapTexture { get; set; }
        public List<Rectangle> MinimapCharacterSourceRectangles { get; set; }
        public List<Rectangle> MinimapArtefactSourceRectangles { get; set; }

        public int NumberOfPlayers { get; set; }

        public Entity[,] MapTiles { get; set; } 

        public Mode GameMode { get; set; }



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
            this.World = new World(Vector2.Zero, Vector2.One);
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
            singleton.ParticleEmitterSystem.Update(gameTime);

            singleton.WorldEventSystem.Update(gameTime);

            if (singleton.IsEarthquake)
            {
                if (singleton.shakeCooldown <= 0)
                {
                    singleton.shakeCooldown = 0.05f;
                    if (singleton.shakedLeft)
                    {
                        singleton.earthQuakeOffset = 5 * Vector2.One;
                    }
                    else
                    {
                        singleton.earthQuakeOffset = -5 * Vector2.One;
                    }
                    singleton.shakedLeft = !singleton.shakedLeft;
                }
                else
                {
                    singleton.shakeCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            singleton.SpriteSystem.Viewport = singleton.Viewport;
            if (singleton.IsEarthquake)
            {
                singleton.World.Origin += singleton.earthQuakeOffset;
            }
            if (!singleton.PauseDrawing)
            {
                singleton.SpriteSystem.Draw(gameTime);
            }
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
            singleton.NumberOfPlayers = gameStartDescription.NumberOfPlayers;
            singleton.GameMode = gameStartDescription.GameMode;
            var content = singleton.gameScreenManager.Game.Content;

            singleton.CharacterTextures = new List<Texture2D>()
            {
                content.Load<Texture2D>(@"Textures\Characters\animated_explorer_spritesheet_red"),
                content.Load<Texture2D>(@"Textures\Characters\animated_explorer_spritesheet_green"),
                content.Load<Texture2D>(@"Textures\Characters\animated_explorer_spritesheet_blue"),
                content.Load<Texture2D>(@"Textures\Characters\animated_explorer_spritesheet_purple")
            };
            singleton.GhostCharacterTextures = new List<Texture2D>()
            {
                content.Load<Texture2D>(@"Textures\Characters\explorer_ghost_red"),
                content.Load<Texture2D>(@"Textures\Characters\explorer_ghost_green"),
                content.Load<Texture2D>(@"Textures\Characters\explorer_ghost_blue"),
                content.Load<Texture2D>(@"Textures\Characters\explorer_ghost_purple")
            };
            singleton.ArtefactTextures = new List<Texture2D>()
            {
                content.Load<Texture2D>(@"Textures\Objects\Artefacts\red_artefact"),
                content.Load<Texture2D>(@"Textures\Objects\Artefacts\green_artefact"),
                content.Load<Texture2D>(@"Textures\Objects\Artefacts\blue_artefact"),
                content.Load<Texture2D>(@"Textures\Objects\Artefacts\purple_artefact")
            };

            singleton.WorldEffectTexture = content.Load<Texture2D>(@"Textures\Maps\animated_event_activated_sprite");

            singleton.WorldEffectAnimation = new AnimationClip(AnimationClipType.Flexing, 2, 400, new Point(1200, 1200));
            singleton.WorldEffectAnimation.SourceRectangle = new Rectangle(0, 0, 1200, 1200);

            singleton.MinimapTexture = content.Load<Texture2D>(@"Textures\Minimap\minimap_sprites");
            singleton.MinimapCharacterSourceRectangles = new List<Rectangle>()
            {
                new Rectangle(0, 0, 200, 200),
                new Rectangle(200, 0, 200, 200),
                new Rectangle(400, 0, 200, 200),
                new Rectangle(600, 0, 200, 200),
            };
            singleton.MinimapArtefactSourceRectangles = new List<Rectangle>()
            {
                new Rectangle(0, 200, 200, 200),
                new Rectangle(200, 200, 200, 200),
                new Rectangle(400, 200, 200, 200),
                new Rectangle(600, 200, 200, 200),
            };

            singleton.Visibility = Visibility.Game;

            singleton.CollisionSystem = new CollisionSystem();
            singleton.SpriteSystem = new SpriteSystem(screenManager.SpriteBatch);
            singleton.PlayerInputSystem = new InputSystem(screenManager);
            singleton.GameplayEffectSystem = new GameplayEffectSystem();
            singleton.AnubisAISystem = new AISystem(AnubisBehaviour.TrueAI);
            singleton.DiscoverySystem = new DiscoverySystem();
            singleton.AnimationSystem = new AnimationSystem();
            singleton.MovementSystem = new MovementSystem();
            singleton.ButtonControllerSystem = new ButtonControllerSystem();
            singleton.ParticleEmitterSystem = new ParticleEmitterSystem();
            singleton.WorldEventSystem = new WorldEventSystem();

            //// set up the initial map
            ChangeMap(gameStartDescription.MapContentName);
            EntityGenerator.Initialize(singleton.gameScreenManager.Game.Content);
            List<Entity> entities = singleton.GenerateMap();
            singleton.World.AddChildren(entities);

            List<Entity> mapEntities = singleton.CreateMapTileEntities();
            singleton.World.AddChildren(mapEntities);
        }

        public static void RegenerateMap()
        {
            singleton.Map.Reset();
            List<Type> notRegeneratedEntities = new List<Type>() { typeof(Character), typeof(Ghost), typeof(Anubis), typeof(HidingCloak) };
            EntityGenerator.DoNotSpawnTypes = notRegeneratedEntities;
            List<Entity> entities = singleton.GenerateMap();
            EntityGenerator.DoNotSpawnTypes = new List<Type>();

            List<Artefact> oldArtefacts = singleton.World.GetChildrenOfType<Artefact>();
            List<Artefact> newArtefacts = entities.FindAll(x => x.GetType() == typeof(Artefact)).Cast<Artefact>().ToList(); ;

            // Remove already collected artefacts from the new map
            List<Artefact> toDelete = new List<Artefact>();
            foreach (Artefact a in newArtefacts)
            {
                bool found = false;
                foreach (Artefact b in oldArtefacts)
                {
                    if (a.GetComponent<Player>().PlayerID == b.GetComponent<Player>().PlayerID)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    toDelete.Add(a);
                }
            }
            foreach (Artefact a in toDelete)
            {
                a.Delete();
                entities.Remove(a);
            }

            // Copy placed artefacts to the new altar
            Altar oldAltar = singleton.World.GetChildrenOfType<Altar>()[0];
            Altar newAltar = (Altar)entities.Find(x => x.GetType() == typeof(Altar));
            int i = 0;
            foreach (InventorySlot slot in oldAltar.GetComponent<Inventory>().ArtefactSlots)
            {
                if (!slot.IsEmpty() && slot.SlotType == SlotType.ArtefactSlot)
                {
                    newAltar.GetComponent<Inventory>().AddArtefact(i);
                    float artefactScale = 0.1f;
                    Texture2D artefactTexture = Session.GetInstance().ArtefactTextures[i];

                    Vector2 altarSize = newAltar.SpriteSize();
                    float artefactWidth = artefactTexture.Width * artefactScale;

                    float w = altarSize.X;
                    float h = altarSize.Y;
                    Vector2[] artefactPositions = new Vector2[]
                    {
                    new Vector2(0.05f*w, -0.17f*h),
                    new Vector2(0.93f*w - artefactWidth, -0.17f*h),
                    new Vector2(0.02f*w, 0.14f*h),
                    new Vector2(0.98f*w - artefactWidth, 0.14f*h)
                    };

                    Artefact artefact = new Artefact(i, artefactPositions[i], Vector2.One * artefactScale, artefactTexture, false);
                    newAltar.AddChild(artefact);
                }
                i++;
            }


            singleton.World.DeleteChildrenExcept(notRegeneratedEntities);

            List<Type> spawnBlockingTypes = new List<Type>()
            {
                typeof(Altar), typeof(Button), typeof(Trap), typeof(Dispenser), typeof(Anubis), typeof(Artefact), typeof(Character)
            };
            List<Point> blockedTiles = new List<Point>();
            foreach (Entity entity in entities.FindAll(x => spawnBlockingTypes.Contains(x.GetType())))
            {
                blockedTiles.Add(singleton.Map.PositionToTileCoordinate(entity.CenterPosition()));
            }

            foreach (Character character in singleton.World.GetChildrenOfType<Character>())
            {
                Point characterTileCoord = singleton.Map.PositionToTileCoordinate(character.CenterPosition());
                Point closestFloorTileToCharacter = singleton.Map.FindClosestFloor(character.CenterPosition(), blockedTiles);
                character.GetComponent<Transform>().Position = singleton.Map.CreateEntityTileCenteredPosition(character.GetComponent<Sprite>().SourceRectangle, character.GetComponent<Transform>().Scale, closestFloorTileToCharacter); ;
            }


            Anubis anubis = singleton.World.GetChildrenOfType<Anubis>()[0];
            Point anubisTileCoord = singleton.Map.PositionToTileCoordinate(anubis.CenterPosition());
            Point closestFloorTileToAnubis = singleton.Map.FindClosestFloor(anubis.CenterPosition(), blockedTiles);
            anubis.GetComponent<Transform>().Position = singleton.Map.CreateEntityTileCenteredPosition(anubis.GetComponent<Sprite>().SourceRectangle, anubis.GetComponent<Transform>().Scale, closestFloorTileToAnubis); ;

            anubis.GetComponent<AI>().MovementGraph = new MovementGraph(singleton.Map);
            singleton.World.AddChildren(entities);

            List<Entity> mapEntities = singleton.CreateMapTileEntities();
            singleton.World.AddChildren(mapEntities);
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
                ClearSystems();

                // Reset map
                singleton.Map.Reset();

                // clear the singleton
                singleton = null;

                if (gameplayScreen != null)
                {
                    gameplayScreen.ExitScreen();
                }
            }
        }

        public static void ClearSystems()
        {
            SpriteSystem.Clear();
            CollisionSystem.Clear();
            GameplayEffectSystem.Clear();
            InputSystem.Clear();
            AISystem.Clear();
            DiscoverySystem.Clear();
            AnimationSystem.Clear();
            MovementSystem.Clear();
            ButtonControllerSystem.Clear();
            ParticleEmitterSystem.Clear();
            WorldEventSystem.Clear();
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
            singleton.World.Scale = singleton.Map.WorldScale * SplitScreen.WorldScaleFactorsBasedOnNPlayers[singleton.NumberOfPlayers - 1];
        }

        public static void StartMinimapMode(Vector2 minimapSize, GameTime gameTime)
        {
            Vector2 scale = minimapSize / singleton.Map.MapSize;
            singleton.Visibility = Visibility.Minimap;

            Vector2 viewportCenter = new Vector2(singleton.viewport.Width / 2f, singleton.viewport.Height / 2f);
            singleton.World.Scale = scale;
            Vector2 mapSize = singleton.Map.MapSize * scale;

            Vector2 topRightMapCenter = new Vector2(
                singleton.viewport.X + singleton.viewport.Width - mapSize.X / 2 - 20,
            singleton.viewport.Y + mapSize.Y / 2 + 20
                );

            Vector2 bottomRightMapCenter = new Vector2(
                singleton.viewport.X + singleton.viewport.Width / 2 + 1 + (singleton.viewport.Width / 2 - 1) / 2,
                singleton.viewport.Y + singleton.viewport.Height /2 + 1 + (singleton.viewport.Height / 2 - 1) / 2

                );
            if (singleton.NumberOfPlayers == 1)
            {
                MoveMapCenterTo(topRightMapCenter);
            }
            else if(singleton.NumberOfPlayers == 3)
            {
                MoveMapCenterTo(bottomRightMapCenter);
            }else
            {
                MoveMapCenterTo(viewportCenter);
            }
            singleton.AnimationSystem.Update(gameTime);
        }
        public static void EndMinimapMode()
        {
            singleton.Visibility = Visibility.Game;
            singleton.World.Scale = singleton.Map.WorldScale * SplitScreen.WorldScaleFactorsBasedOnNPlayers[singleton.NumberOfPlayers - 1];
        }

        public static void SetViewport(Viewport viewport)
        {
            singleton.Viewport = viewport;
        }

        public static void SetFocusOnPlayer(int playerIdx)
        {
            foreach (Input playerInput in InputSystem.GetComponents())
            {
                if (playerInput.Entity.GetComponent<Player>().PlayerID == playerIdx)
                {
                    Entity player = playerInput.Entity;
                    if(player.GetType() == typeof(Character))
                    {
                        Character character = (Character)player;
                        if(character.Ghost != null)
                        {
                            continue;
                        }
                    }
                    Vector2 playerCenter = singleton.World.Scale * player.CenterPosition();

                    singleton.World.Origin = singleton.viewportCenter - playerCenter;
                }
            }
        }
        public static void MoveMapCenterTo(Vector2 newMapCenter)
        {
            Vector2 mapCenter = new Vector2(
                singleton.Map.MapDimensions.X * singleton.Map.TileSize.X * singleton.World.Scale.X / 2,
                singleton.Map.MapDimensions.Y * singleton.Map.TileSize.Y * singleton.World.Scale.Y / 2);
            singleton.World.Origin = newMapCenter - mapCenter;
        }
        public List<Entity> CreateMapTileEntities()
        {
            Random random = new Random();
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
                            Wall newWall = new Wall(position, singleton.Map.TileScale, singleton.Map.Texture, singleton.Map.UndiscoveredTexture, sourceRectangle, true);
                            entities.Add(newWall);
                            singleton.MapTiles[x, y] = newWall;
                            AddWallCorners(newWall, mapPosition);
                            if(random.NextDouble() < singleton.Map.TorchProbability)
                            {
                                TryAddTorch(newWall, mapPosition);
                            }

                        }
                        else
                        {
                            Floor newFloor = new Floor(position,  singleton.Map.TileScale, singleton.Map.Texture, singleton.Map.UndiscoveredTexture, sourceRectangle);
                            entities.Add(newFloor);
                            singleton.MapTiles[x, y] = newFloor;
                            if (random.NextDouble() < singleton.Map.TorchProbability)
                            {
                                TryAddTorch(newFloor, mapPosition);
                            }
                        }
                    }
                }
            }
            AddWallBorder(4);
            return entities;
        }

        public void AddWallCorners(Wall wall, Point mapPosition)
        {
            List<Rectangle> wallCorners = singleton.Map.GetWallCornerRectangles();

            int baseLayerValue = singleton.Map.GetBaseLayerValue(mapPosition);

            // Wall candidates for which the corners are an option
            int[] upLeftLayerValues = new int[] { 6, 11, 13, 15 };
            int[] upRightLayerValues = new int[] { 7, 12, 13, 15 };
            int[] downLeftLayerValues = new int[] { 8, 11, 14, 15 };
            int[] downRightLayerValues = new int[] { 9, 12, 14, 15 };


            Point upLeft = mapPosition + new Point(-1, -1);
            Point upRight = mapPosition + new Point(1, -1);
            Point downLeft = mapPosition + new Point(-1, 1);
            Point downRight = mapPosition + new Point(1, 1);

            if(singleton.Map.ValidTileCoordinates(upLeft) && singleton.Map.GetCollisionLayerValue(upLeft)  == 0 && upLeftLayerValues.Contains(baseLayerValue))
            {
                Sprite sprite = new Sprite(singleton.Map.Texture, wallCorners[0], 1, Visibility.Game);
                wall.AddComponent(sprite);
            }
            if (singleton.Map.ValidTileCoordinates(upRight) && singleton.Map.GetCollisionLayerValue(upRight) == 0 && upRightLayerValues.Contains(baseLayerValue))
            {
                Sprite sprite = new Sprite(singleton.Map.Texture, wallCorners[1], 1, Visibility.Game);
                wall.AddComponent(sprite);
            }
            if (singleton.Map.ValidTileCoordinates(downLeft) && singleton.Map.GetCollisionLayerValue(downLeft) == 0 && downLeftLayerValues.Contains(baseLayerValue))
            {
                Sprite sprite = new Sprite(singleton.Map.Texture, wallCorners[2], 1, Visibility.Game);
                wall.AddComponent(sprite);
            }
            if (singleton.Map.ValidTileCoordinates(downRight) && singleton.Map.GetCollisionLayerValue(downRight) == 0 && downRightLayerValues.Contains(baseLayerValue))
            {
                Sprite sprite = new Sprite(singleton.Map.Texture, wallCorners[3], 1, Visibility.Game);
                wall.AddComponent(sprite);
            }

        }

        public void TryAddTorch(Wall wall, Point mapPosition)
        {
            Transform transform = wall.GetComponent<Transform>();
            Point down = mapPosition + new Point(0, 1);
            Point right = mapPosition + new Point(1, 0);

            if (singleton.Map.ValidTileCoordinates(down) && singleton.Map.GetCollisionLayerValue(down) == 0)
            {
                Torch torch = new Torch(transform.Position, transform.Scale, singleton.Map.TorchTexture, singleton.Map.TorchSourceRectangles[0]);
                singleton.World.AddChild(torch);
            }
            if (singleton.Map.ValidTileCoordinates(right) && singleton.Map.GetCollisionLayerValue(right) == 0)
            {
                Torch torch = new Torch(transform.Position, transform.Scale, singleton.Map.TorchTexture, singleton.Map.TorchSourceRectangles[2]);
                singleton.World.AddChild(torch);
            }

        }
        public void TryAddTorch(Floor floor, Point mapPosition)
        {
            Transform transform = floor.GetComponent<Transform>();
            Point down = mapPosition + new Point(0, 1);
            Point right = mapPosition + new Point(1, 0);
            if (singleton.Map.ValidTileCoordinates(down) && singleton.Map.GetCollisionLayerValue(down) == 1)
            {
                Torch torch = new Torch(transform.Position, transform.Scale, singleton.Map.TorchTexture, singleton.Map.TorchSourceRectangles[1]);
                singleton.World.AddChild(torch);
            }
            if (singleton.Map.ValidTileCoordinates(right) && singleton.Map.GetCollisionLayerValue(right) == 1)
            {
                Torch torch = new Torch(transform.Position, transform.Scale, singleton.Map.TorchTexture, singleton.Map.TorchSourceRectangles[3]);
                singleton.World.AddChild(torch);
            }
        }

        public void AddWallBorder(int borderSize)
        {
            Point mapDim = singleton.Map.MapDimensions;
            Rectangle sourceRectangle = singleton.Map.GetBaseLayerSourceRectangle(15);

            for (int x = -borderSize; x < mapDim.X + borderSize; x++)
            {
                for (int y = -borderSize; y < mapDim.Y + borderSize; y++)
                {
                    if (x < 0 || x >= mapDim.X || y < 0 || y >= mapDim.Y)
                    {
                        Vector2 position = new Vector2(x * singleton.Map.TileSize.X, y * singleton.Map.TileSize.Y);
                        Wall newWall = new Wall(position, singleton.Map.TileScale, singleton.Map.Texture, singleton.Map.UndiscoveredTexture, sourceRectangle, false);
                        singleton.World.AddChild(newWall);
                    }
                }
            }
        }

        public List<Entity> GenerateMap()
        {
            singleton.Map.MapBlocks = new List<MapBlock>();
            foreach (MapBlockDescription mapBlockDesc in singleton.Map.MapBlockDescriptions)
            {
                mapBlockDesc.BasePriority = mapBlockDesc.Priority;
                mapBlockDesc.Blocks = new List<MapBlock>();
                if (mapBlockDesc.OccursNPlayerOften)
                {
                    mapBlockDesc.MinOccurences = NumberOfPlayers;
                    mapBlockDesc.MaxOccurences = NumberOfPlayers;
                }
                foreach (string mapBlockName in mapBlockDesc.BlockNames)
                {
                    MapBlock block = singleton.gameScreenManager.Game.Content.Load<MapBlock>(@"Maps\MapBlocks\" + mapBlockName);
                    block.Name = mapBlockName;
                    block.Parent = mapBlockDesc;
                    if(singleton.NumberOfPlayers >= block.MinPlayers && singleton.NumberOfPlayers <= block.MaxPlayers)
                    {
                        mapBlockDesc.Blocks.Add(block);
                        singleton.Map.MapBlocks.Add(block);
                    }
                }
            }
            MapGenerator gen = new MapGenerator(singleton.Map);
            List<EntityDescription> entityDescs = gen.GenerateMap();
            return EntityGenerator.GenerateEntities(entityDescs);
        }
    }
}
