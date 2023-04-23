using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.GameScreens
{
    class LoginPlayersScreen : GameScreen
    {
        private GameStartDescription gameStartDescription;

        private int startCoolDown;

        private List<Texture2D> keyboardInput;
        private Texture2D controllerInput;
        private Texture2D joinInstruction;

        private List<Texture2D> playerSprites;
        private List<PlayerInput> joinedPlayers;

        private string startDescription;
        private Texture2D startButtonTexture;

        private Texture2D lineTexture;
        private Rectangle verticalLinePos, horizontalLinePos;
        private int playerFrameWidth, playerFrameHeight;

        // Constants
        private Color lineColor = Color.White;
        private Color buttonColor = Color.LimeGreen;

        private SpriteFont descriptionFont = Fonts.CascadiaFont;

        private int lineThickness = 2;

        // Sprite Scales
        private float spriteScale = 0.7f;
        private float keyboardScale = 0.4f, controllerScale = 0.3f;
        private float instructionScale = 0.4f;

        // Relative to playerFrame size
        private float marginX = 0.03f, marginY = 0.03f;
        private float startButtonHeightScale = 0.1f;


        public LoginPlayersScreen() : base()
        {
            gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map001";
            gameStartDescription.NumberOfPlayers = InputController.GetActiveInputs().Count;

            joinedPlayers = InputController.GetActiveInputs();
            startCoolDown = 250;
        }

        public override void LoadContent()
        {
            ContentManager content = GameScreenManager.Game.Content;
            Texture2D playerOne = content.Load<Texture2D>("Textures/Menu/LoginScreen/explorer_red");
            Texture2D playerTwo = content.Load<Texture2D>("Textures/Menu/LoginScreen/explorer_green");
            Texture2D playerThree = content.Load<Texture2D>("Textures/Menu/LoginScreen/explorer_blue");
            Texture2D playerFour = content.Load<Texture2D>("Textures/Menu/LoginScreen/explorer_purple");
            playerSprites = new List<Texture2D> { playerOne, playerTwo, playerThree, playerFour };

            Texture2D inputOne = content.Load<Texture2D>("Textures/Menu/LoginScreen/Keyboard1");
            Texture2D inputTwo = content.Load<Texture2D>("Textures/Menu/LoginScreen/Keyboard2");
            Texture2D inputThree = content.Load<Texture2D>("Textures/Menu/LoginScreen/Keyboard3");
            Texture2D inputFour = content.Load<Texture2D>("Textures/Menu/LoginScreen/Keyboard4");
            keyboardInput = new List<Texture2D> { inputOne, inputTwo, inputThree, inputFour };

            controllerInput = content.Load<Texture2D>("Textures/Menu/LoginScreen/Controller");
            joinInstruction = content.Load<Texture2D>("Textures/Menu/LoginScreen/JoinInstruction");

            Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;
            int totalWidth = viewport.Width;
            int totalHeight = viewport.Height;

            lineTexture = new Texture2D(GameScreenManager.GraphicsDevice, 1, 1);
            lineTexture.SetData(new[] { lineColor });

            playerFrameWidth = (int)((totalWidth - lineThickness) / 2);
            playerFrameHeight = (int)((totalHeight - lineThickness) / 2);

            verticalLinePos = new Rectangle(playerFrameWidth, 0, lineThickness, totalHeight);
            horizontalLinePos = new Rectangle(0, playerFrameHeight, totalWidth, lineThickness);

            startButtonTexture = new Texture2D(GameScreenManager.GraphicsDevice, 1,1);
            startButtonTexture.SetData(new[] { buttonColor });
            startDescription = "Start With 1 Player";
        }

        public override void HandleInput()
        {
            base.HandleInput();
            foreach (PlayerInput playerInput in InputController.PlayerInputs)
            {
                if (playerInput.UseTriggered() && !playerInput.IsActive)
                {
                    joinedPlayers = InputController.GetActiveInputs();
                    int activeInputs = joinedPlayers.Count();
                    if (activeInputs < 4)
                    {
                        playerInput.IsActive = true;
                        playerInput.PlayerID = activeInputs;
                        gameStartDescription.NumberOfPlayers = activeInputs + 1;
                        
                        if (activeInputs == 0)
                        {
                            startDescription = "Start With 1 Player";
                        }
                        else
                        {
                            startDescription = "Start With " + gameStartDescription.NumberOfPlayers + " Players";
                        }
                        joinedPlayers = InputController.GetActiveInputs();
                    }
                }
            }

            // Start the game if the first player pressed the use button
            if (joinedPlayers[0].UseTriggered() && startCoolDown <= 0)
            {
                LoadingScreen.Load(GameScreenManager, true, new GameplayScreen(gameStartDescription));
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            startCoolDown -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(lineTexture, verticalLinePos, Color.White);
            spriteBatch.Draw(lineTexture, horizontalLinePos, Color.White);

            for (int i = 0; i < 4; i++)
            {
                Rectangle playerFrame;

                switch (i)
                {
                    case 0: playerFrame = new Rectangle(0, 0, playerFrameWidth, playerFrameHeight); break;
                    case 1: playerFrame = new Rectangle(playerFrameWidth + lineThickness, 0, playerFrameWidth, playerFrameHeight); break;
                    case 2: playerFrame = new Rectangle(0, playerFrameHeight + lineThickness, playerFrameWidth, playerFrameHeight); break;
                    case 3: playerFrame = new Rectangle(playerFrameWidth + lineThickness, playerFrameHeight + lineThickness, playerFrameWidth, playerFrameHeight);
                        break;
                    default: playerFrame = new Rectangle(0, 0, playerFrameWidth, playerFrameHeight); break;
                }

                if (i < joinedPlayers.Count())
                {
                    string playerNumber = "Player " + (i + 1);
                    PlayerInput controls = joinedPlayers[i];
                    Texture2D playerSprite = playerSprites[i];
                    Texture2D playerControl;
                    float playerControlScale;

                    if (controls.IsKeyboard)
                    {
                        Keys useKey = controls.UseKey;
                        playerControlScale = keyboardScale;

                        switch (useKey)
                        {
                            case Keys.E: playerControl = keyboardInput[0]; break;
                            case Keys.Z: playerControl = keyboardInput[1]; break;
                            case Keys.O: playerControl = keyboardInput[2]; break;
                            case Keys.OemMinus: playerControl = keyboardInput[3]; break;
                            default: playerControl = keyboardInput[0];  break;
                        }
                    }
                    else
                    {
                        playerControl = controllerInput;
                        playerControlScale = controllerScale;
                    }

                    DrawOnlinePlayerFrame(playerNumber, playerFrame, playerSprite, playerControl, playerControlScale);
                }
                else
                {
                    DrawOfflinePlayerFrame(playerFrame);
                }   
            }

            spriteBatch.End();
        }

        // Draw join instructions for empty player frames
        public void DrawOfflinePlayerFrame(Rectangle playerFrame)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            int textureWidth = (int)(joinInstruction.Width * instructionScale);
            int textureHeight = (int)(joinInstruction.Height * instructionScale);
            Vector2 instructionPosition = new Vector2(playerFrame.X, playerFrame.Y) 
                + new Vector2((playerFrame.Width - textureWidth)/2, (playerFrame.Height - textureHeight)/2);
            spriteBatch.Draw(joinInstruction, instructionPosition, null, Color.White, 0f, Vector2.Zero, instructionScale, SpriteEffects.None, 0f);

        }

        // Draw player sprites and controls for joined players
        public void DrawOnlinePlayerFrame(string playerNumber, Rectangle playerFrame, Texture2D playerSprite, Texture2D playerControl, float playerControlScale)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            Vector2 textLength = descriptionFont.MeasureString(playerNumber);
            int topMargin = (int) (marginY * playerFrameHeight);
            Vector2 textPosition = new Vector2(playerFrame.X + (playerFrame.Width - textLength.X) / 2, playerFrame.Y + topMargin);
            spriteBatch.DrawString(descriptionFont, playerNumber, textPosition, Color.White);

            int spriteWidth = (int)(playerSprite.Width * spriteScale);
            int spriteHeight = (int)(playerSprite.Height * spriteScale);
            int playerControlWidth = (int)(playerControl.Width * playerControlScale);
            int playerControlHeight = (int)(playerControl.Height * playerControlScale);
            int offSetX = (int)(playerFrame.Width - (spriteWidth + playerControlWidth)) / 3;
            int offsetY = (int)(playerFrame.Height - (textLength.Y + 2 * topMargin + spriteHeight)) / 2;
            Vector2 spritePosition = new Vector2(playerFrame.X + offSetX, playerFrame.Y + offsetY);

            offsetY = (int)(playerFrame.Height - (textLength.Y + 2 * topMargin + playerControlHeight)) / 2;
            Vector2 playerControlPosition =  new Vector2(spritePosition.X + offSetX + spriteWidth, playerFrame.Y + offsetY);

            spriteBatch.Draw(playerSprite, spritePosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(playerControl, playerControlPosition, null, Color.White, 0f, Vector2.Zero, playerControlScale, SpriteEffects.None, 0f);

            // Draw the start button only for player 1
            if(playerNumber.Equals("Player 1"))
            {
                offsetY = (int)(playerFrame.Y + (1.0f - startButtonHeightScale) * playerFrame.Height);
                int startButtonHeight = (int)(startButtonHeightScale * playerFrame.Height);
                Rectangle startButtonPosition = new Rectangle(playerFrame.X, offsetY, playerFrame.Width, startButtonHeight);
                spriteBatch.Draw(startButtonTexture, startButtonPosition, Color.White);

                textLength = descriptionFont.MeasureString(startDescription);
                textPosition = new Vector2(playerFrame.X + (playerFrame.Width - textLength.X) / 2, offsetY + (startButtonHeight - textLength.Y)/2);
                spriteBatch.DrawString(descriptionFont, startDescription, textPosition, Color.White);
            }
        }
    }
}
