using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace TombOfAnubis
{
    public class Hud
    {
        private Texture2D minimapBackground;
        private Texture2D minimapFrame;
        private float minimapScale = 0.25f;
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
        private Dictionary<ItemType, Texture2D> itemTextures = new Dictionary<ItemType, Texture2D>();
        private float itemDisplayScale = 0.3f;
        private float artefactDisplayScale = 0.4f;


        public Hud(GraphicsDevice graphicsDevice, GameScreenManager gameScreenManager)
        {
            graphics = graphicsDevice;
            viewport = graphics.Viewport;
            session = Session.GetInstance();
            characters = session.World.GetChildrenOfType<Character>();
            characterViewports = SplitScreen.PlayerViewports;
            minimapBackground = new Texture2D(graphicsDevice, 1, 1);
            minimapBackground.SetData(new[] { Color.Black });



            // In-game UI Fonts
            statusFont = Fonts.DisneyHeroicFont;
            statusColor = Fonts.ArtefactStatusColor;
            screenManager = gameScreenManager;

            LoadContent();
        }

        public void LoadContent()
        {
            ContentManager content = screenManager.Game.Content;
            minimapBackground = content.Load<Texture2D>("Textures/Menu/treasuremap_background");
            minimapFrame = content.Load<Texture2D>("Textures/Menu/treasuremap_border");
            for (int i = 0; i < characters.Count; i++)
            {
                string textureName = "Item_slot_" + (i + 1);
                string textureFullPath = "Textures/Objects/UI/" + textureName;
                Texture2D itemSlotTexture = content.Load<Texture2D>(textureFullPath);
                itemBorderTextures.Add(itemSlotTexture);
            }

            foreach (int i in Enum.GetValues(typeof(ItemType)))
            {
                //can load textures directly from the ItemTextureLibrary
                itemTextures.Add((ItemType)i, ItemTextureLibrary.GetTexture((ItemType)i));             
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
            Vector2 minimapSize = new Vector2(minimapScale * viewport.Height, minimapScale * viewport.Height); 
            Session.StartMinimapMode(minimapSize);

            Vector2 minimapPosition = session.World.Origin;

            //Minimap background
            session.SpriteSystem.SpriteBatch.Draw(minimapBackground, new Rectangle((int)minimapPosition.X - 20, (int)minimapPosition.Y - 20, (int)minimapSize.X + 40, (int)minimapSize.Y + 40), Color.White);
            Session.Draw(gameTime);
            session.SpriteSystem.SpriteBatch.Draw(minimapFrame, new Rectangle((int)minimapPosition.X - 20, (int)minimapPosition.Y - 20, (int)minimapSize.X + 40, (int)minimapSize.Y + 40), Color.White);


            Session.EndMinimapMode();
        }

        private void DrawArtefactInventory(GameTime gameTime, Character character, Viewport characterViewport)
        {
            // TODO: Implement

            int playerID = character.GetComponent<Player>().PlayerID;
            bool hasArtefact = character.GetComponent<Inventory>().HasArtefact();

            bool hasPowerUp = false;
            ItemType powerUp = ItemType.None;
            if (character.GetComponent<Inventory>().GetEmptyItemSlot() == null)
            {
                hasPowerUp = true;
                powerUp = character.GetComponent<Inventory>().ItemSlots[0].Item.ItemType;
                
            }
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
                // Fill up artefact slot with a filler string
                DrawFillerString(artefactSlotFiller, textureWidth, textureHeight, positionX[0], positionY[0], 0);
            }

            if(hasPowerUp)
            {
                if (itemTextures.ContainsKey(powerUp))
                {
                    texture = itemTextures[powerUp];
                    DrawItemSprite(playerID, texture, textureWidth, textureHeight, positionX[0], positionY[0]);
                }
            }

            else
            {
                // Fill up item slot with a filler string
                DrawFillerString(itemSlotFiller, textureWidth, textureHeight, positionX[0], positionY[0], 1);
            }
        }

        /// <summary>
        /// Draws the artefact sprite in the designated slot
        /// </summary>
        private void DrawArtefactSprite(int playerID, int frameWidth, int frameHeight, int framePositionX, int framePositionY)
        {
            Texture2D artefactTexture = session.Map.Artefacts[playerID].Texture;
            Vector2 artefactScale = session.Map.Artefacts[playerID].Scale;

            int artefactWidth = (int)(artefactTexture.Width * artefactScale.X * artefactDisplayScale);
            int artefactHeight = (int)(artefactTexture.Height * artefactScale.Y * artefactDisplayScale);

            Vector2 artefactPositionOffSet = new Vector2((frameWidth - artefactWidth) / 4, (frameHeight - artefactHeight) / 2);
            Vector2 artefactDisplayPosition = new Vector2(framePositionX, framePositionY) + artefactPositionOffSet;

            Rectangle DestinationRectangle = new Rectangle((int)artefactDisplayPosition.X, (int)artefactDisplayPosition.Y, artefactWidth, artefactHeight);

            session.SpriteSystem.SpriteBatch.Draw(artefactTexture, DestinationRectangle, Color.White);
        }

        /// <summary>
        /// Draws the artefact sprite in the designated slot
        /// </summary>
        private void DrawItemSprite(int playerID, Texture2D itemTexture, int frameWidth, int frameHeight, int framePositionX, int framePositionY)
        {
            int itemWidth = (int)(itemTexture.Width * itemDisplayScale);
            int itemHeight = (int)(itemTexture.Height * itemDisplayScale);

            Vector2 itemPositionOffSet = new Vector2((frameWidth - itemWidth) * 3 / 4, (frameHeight - itemHeight) / 2);
            Vector2 itemDisplayPosition = new Vector2(framePositionX, framePositionY) + itemPositionOffSet;

            Rectangle DestinationRectangle = new Rectangle((int)itemDisplayPosition.X, (int)itemDisplayPosition.Y, itemWidth, itemHeight);

            session.SpriteSystem.SpriteBatch.Draw(itemTexture, DestinationRectangle, Color.White);
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
