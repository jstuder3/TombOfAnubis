using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace TombOfAnubis
{
    public class Hud
    {
        public Vector2 MinimapScale {  get; set; }
        private Texture2D minimapBackground;
        private Viewport viewport;
        private GraphicsDevice graphics;
        private Session session;
        private List<Character> characters;
        private List<Viewport> characterViewports;
        private SpriteFont statusFont;
        private Color statusColor;
        private GameScreenManager screenManager;
        private List<Texture2D> itemBorderTextures = new List<Texture2D>();
        private int displayOffset = 30;
        private int placementOffset = 3;
        private float scalingFactor = 0.75f;
        private int numSlots = 1;
        private float fontScale = 0.35f;
        private string artefactSlotFiller = "Artefact";
        private string itemSlotFiller = "Power Up";


        public Hud(GraphicsDevice graphicsDevice, GameScreenManager gameScreenManager)
        {
            graphics = graphicsDevice;
            viewport = graphics.Viewport;
            session = Session.GetInstance();
            MinimapScale = Vector2.One / 16;
            characters = session.Scene.GetChildrenOfType<Character>();
            characterViewports = SplitScreen.PlayerViewports;
            minimapBackground = new Texture2D(graphicsDevice, 1, 1);
            minimapBackground.SetData(new[] { Color.Yellow });

            // In-game UI Fonts
            statusFont = Fonts.DisneyHeroicFont;
            statusColor = Fonts.ArtefactStatusColor;
            screenManager = gameScreenManager;

            LoadContent();
        }

        public void LoadContent()
        {
            ContentManager content = screenManager.Game.Content;
            for (int i = 0; i < characters.Count; i++)
            {
                string textureName = "Item_slot_" + (i + 1);
                string textureFullPath = "Textures/Objects/UI/" + textureName;
                Texture2D itemSlotTexture = content.Load<Texture2D>(textureFullPath);
                itemBorderTextures.Add(itemSlotTexture);
            }

        }

        public void Draw(GameTime gameTime)
        {

            DrawMinimap(gameTime);

            for (int i = 0; i < characters.Count; i++)
            {
                DrawArtefactInventory(gameTime, characters[i], characterViewports[i]);
            }
        }

        private void DrawMinimap(GameTime gameTime)
        {
            session.Scene.GetComponent<Transform>().Scale = MinimapScale;

            Vector2 viewportCenter = new Vector2(
             viewport.Width / 2f,
             viewport.Height / 2f);
            Vector2 mapSize = session.Map.MapSize * MinimapScale;

            Vector2 topRightMapCenter = new Vector2(
                viewport.X + viewport.Width - mapSize.X / 2 - 10,
                viewport.Y + mapSize.Y / 2 + 10
                ) ;

            if(session.NumberOfPlayers > 1)
            {
                Session.MoveMapCenterTo(viewportCenter);
            }
            else
            {
                Session.MoveMapCenterTo(topRightMapCenter);
            }
            Vector2 minimapPosition = session.Scene.GetComponent<Transform>().Position;


            // Minimap background
            session.SpriteSystem.SpriteBatch.Draw(minimapBackground, new Rectangle((int)minimapPosition.X - 1, (int)minimapPosition.Y - 1, (int)mapSize.X + 2, (int)mapSize.Y + 2), Color.White * 0.1f);

            Session.Draw(gameTime);




            session.Scene.GetComponent<Transform>().Scale = Vector2.One;
        }

        private void DrawArtefactInventory(GameTime gameTime, Character character, Viewport characterViewport)
        {
            // TODO: Implement

            int playerID = character.GetComponent<Player>().PlayerID;
            bool hasArtefact = character.GetComponent<Inventory>().HasArtefact();
            // bool hasPowerup = character.GetComponent<Inventory>().GetEmptyItemSlot();
            // int totalArtefacts = character.GetComponent<Inventory>().ArtefactSlots.Count;

            Texture2D texture = itemBorderTextures[playerID];
            int textureWidth = (int) (texture.Width * scalingFactor);
            int textureHeight = (int)(texture.Height * scalingFactor);

            List<int> positionX = new List<int>();
            List<int> positionY = new List<int>();

            for (int i = 0; i < numSlots; i++)
            {
                int X, Y;
                if (playerID == 0)
                {
                    X = characterViewport.X + textureWidth * i + placementOffset;
                    Y = characterViewport.Y + placementOffset;
                }
                else if (playerID == 1)
                {
                    X = characterViewport.X + characterViewport.Width - textureWidth * (i+1) - placementOffset;
                    Y = characterViewport.Y + placementOffset;
                }
                else if (playerID == 2)
                {
                    X = characterViewport.X + textureWidth * i + placementOffset;
                    Y = characterViewport.Y + characterViewport.Height - textureHeight - placementOffset - displayOffset;
                }
                else
                {
                    X = characterViewport.X + characterViewport.Width - textureWidth * (i+1) - placementOffset;
                    Y = characterViewport.Y + characterViewport.Height - textureHeight - placementOffset - displayOffset;
                }

                positionX.Add(X);
                positionY.Add(Y);
            }

            for (int i=0; i < numSlots; i++)
            {
                // Rectangle SourceRectangle = new Rectangle(0,0,texture.Width, texture.Height);
                Rectangle DestinationRectangle = new Rectangle(positionX[i], positionY[i], textureWidth, textureHeight);
                session.SpriteSystem.SpriteBatch.Draw(texture, DestinationRectangle, Color.White);
            }

            if (hasArtefact)
            {
                DrawArtefactSprite(playerID, textureWidth, textureHeight, positionX[0], positionY[0]);
            }

            else
            {
                DrawFillerString(artefactSlotFiller, textureWidth, textureHeight, positionX[0], positionY[0], 0);
            }

            // Fill up item slot with a filler
            DrawFillerString(itemSlotFiller ,textureWidth, textureHeight, positionX[0], positionY[0], 1);
        }

        /// <summary>
        /// Draws the artefact sprite in the designated slot
        /// </summary>
        private void DrawArtefactSprite(int playerID, int frameWidth, int frameHeight, int framePositionX, int framePositionY)
        {
            Texture2D artefactTexture = session.Map.Artefacts[playerID].Texture;
            Vector2 artefactScale = session.Map.Artefacts[playerID].Scale;

            int artefactWidth = (int)(artefactTexture.Width * artefactScale.X);
            int artefactHeight = (int)(artefactTexture.Height * artefactScale.Y);

            Vector2 artefactPositionOffSet = new Vector2((frameWidth - artefactWidth) / 4, (frameHeight - artefactHeight) / 2);
            Vector2 artefactDisplayPosition = new Vector2(framePositionX, framePositionY) + artefactPositionOffSet;

            Rectangle DestinationRectangle = new Rectangle((int)artefactDisplayPosition.X, (int)artefactDisplayPosition.Y, artefactWidth, artefactHeight);

            session.SpriteSystem.SpriteBatch.Draw(artefactTexture, DestinationRectangle, Color.White);
        }

        /// <summary>
        /// Draws a filler string when no artefact or item is collected.
        /// Slot 0 is for an artefact, the other for the item.
        /// </summary>
        private void DrawFillerString(string statusText,int frameWidth, int frameHeight, int framePositionX, int framePositionY, int slot)
        {
            Vector2 textLength = statusFont.MeasureString(statusText) * fontScale;
            Vector2 positionOffSet;
            if (slot == 0)
            {
                positionOffSet = new Vector2((frameWidth - textLength.X) / 4, (frameHeight - textLength.Y) / 2);
            }
            else {
                positionOffSet = new Vector2((frameWidth - textLength.X) * 3 / 4, (frameHeight - textLength.Y) / 2);
            }
            
            Vector2 displayPosition = new Vector2(framePositionX, framePositionY) + positionOffSet;

            session.SpriteSystem.SpriteBatch.DrawString(statusFont, statusText, displayPosition, statusColor,
            0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
        }
    }
}
