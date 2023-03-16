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

        /// <summary>
        /// The list of characters such as players, anubis or other entities.
        /// </summary>
        /// <remarks>The first entry is the leader.</remarks>
        private List<Character> characters = new List<Character>();

        /// <summary>
        /// The list of characters such as players, anubis or other entities.
        /// </summary>
        /// <remarks>The first entry is the leader.</remarks>
        [ContentSerializerIgnore]
        public List<Character> Characters
        {
            get { return characters; }
            set { characters = value; }
        }

        private Map map;
        [ContentSerializerIgnore]
        public Map Map
        {
            get { return map; }
            set { map = value; }
        }


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
            TileEngine.Update(gameTime);

            foreach (Character character in singleton.characters)
            {
                character.Update(gameTime);
            }


        }

        /// <summary>
        /// Draws the session environment to the screen
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = singleton.gameScreenManager.SpriteBatch;


            // draw the background
            spriteBatch.Begin();
            if (TileEngine.Map.Texture != null)
            {
                // draw the ground layer
                TileEngine.DrawLayers(spriteBatch, true, true, false);

                // draw the characters
                foreach (Character character in singleton.characters)
                {
                    character.Draw(gameTime, spriteBatch);
                }
                //// draw the character shadows
                //DrawShadows(spriteBatch);
            }
            spriteBatch.End();
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

            //// set up the initial map
            ChangeMap(gameStartDescription.MapContentName);

            // set up the initial party
            //ContentManager content = singleton.screenManager.Game.Content;
            //singleton.party = new Party(gameStartDescription, content);

            singleton.Characters.Add(new Character(
                PlayerType.Player,
                100,
                100,
                singleton.Map.SpawnMapPosition.X,
                singleton.Map.SpawnMapPosition.Y,
                singleton.gameScreenManager.Game.Content.Load<Texture2D>("Textures/Characters/plagiarized_explorer"),
                1,
                 new InputController()));
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

            // set the new map into the tile engine
            TileEngine.SetMap(singleton.Map);
        }

    }
}
