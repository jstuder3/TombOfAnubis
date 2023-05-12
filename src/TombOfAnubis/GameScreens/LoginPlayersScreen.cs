using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.GameScreens
{

    public enum Mode
    {
        Easy,
        Normal,
    }

    class LoginPlayersScreen : GameScreen
    {
        private GameStartDescription gameStartDescription;

        private Texture2D backgroundTexture;
        private Rectangle backgroundPosition;

        private int startCoolDown;

        private List<Texture2D> keyboardInput;
        private Texture2D controllerInput;
        private Texture2D joinInstruction;

        private List<Texture2D> playerSprites;
        private List<PlayerInput> joinedPlayers;

        private string startDescription;
        private Texture2D startBackground;
        private string modeEasy = "Easy";
        private Texture2D easyButton;
        private string modeNormal = "Normal";
        private Texture2D normalButton;


        private Texture2D lineTexture;
        private Rectangle verticalLinePos, horizontalLinePos;
        private int playerFrameWidth, playerFrameHeight;

        private Texture2D startButton;

        // Constants
        private Color lineColor = Color.White;
        private Color buttonColor = Color.LimeGreen;
        private Color easyColor = Color.DarkGreen;
        private Color normalColor = Color.OrangeRed;

        private SpriteFont descriptionFont = Fonts.CascadiaFont;

        private int lineThickness = 2;

        // Sprite & Font Scales
        private float spriteScale = 1.0f;
        private float keyboardScale = 0.5f, controllerScale = 0.5f;
        private float instructionScale = 0.6f;
        private float playerNumberScale = 1.4f;
        private float startButtonScale = 0.25f;
        private float startTextScale = 0.85f;
        private float modeTextScale = 0.7f;

        // Relative to playerFrame size
        private float marginStartButton = 0.1f;
        private float marginY = 0.03f;
        private float startBarHeightScale = 0.2f;

        // Relative to startButton size
        private float modeButtonWidth = 0.35f, modeButtonHeight = 0.45f;
        private float modeButtonGap = 0.1f;

        // Set default to normal
        private Mode selectedMode = Mode.Normal;

        private bool buttonCooldown;
        private bool buttonPressed;
        private double lastPressed;


        public LoginPlayersScreen() : base()
        {
            gameStartDescription = new GameStartDescription();
            gameStartDescription.MapContentName = "Map001";
            gameStartDescription.NumberOfPlayers = InputController.GetActiveInputs().Count;

            joinedPlayers = InputController.GetActiveInputs();
            startCoolDown = 250;

            buttonCooldown = false;
            buttonPressed = false;
        }

        public override void LoadContent()
        {
            ContentManager content = GameScreenManager.Game.Content;
            backgroundTexture = content.Load<Texture2D>("Textures/Menu/LoginScreen/login_screen_bg");

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

            PlayerInput firstPlayer = InputController.GetActiveInputs()[0];

            switch (firstPlayer.UseKey)
            {
                default: startButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/UseController"); break;

                case Keys.E: startButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/Use1"); break;
                case Keys.Z: startButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/Use2"); break;
                case Keys.O: startButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/Use3"); break;
                case Keys.OemMinus: startButton = content.Load<Texture2D>("Textures/Menu/LoginScreen/Use4"); break;
            }

            //startButton = content.Load<Texture2D>()

            Viewport viewport = ResolutionController.TargetViewport;
            int screenWidth = viewport.Width;
            int screenHeight = viewport.Height;

            backgroundPosition = new Rectangle(0,0, screenWidth, screenHeight);

            lineTexture = new Texture2D(GameScreenManager.GraphicsDevice, 1, 1);
            lineTexture.SetData(new[] { lineColor });

            playerFrameWidth = (int)((screenWidth - lineThickness) / 2);
            playerFrameHeight = (int)((screenHeight - lineThickness) / 2);

            verticalLinePos = new Rectangle(playerFrameWidth, 0, lineThickness, screenHeight);
            horizontalLinePos = new Rectangle(0, playerFrameHeight, screenWidth, lineThickness);

            startBackground = new Texture2D(GameScreenManager.GraphicsDevice, 1,1);
            startBackground.SetData(new[] { buttonColor });
            easyButton = new Texture2D(GameScreenManager.GraphicsDevice, 1, 1);
            easyButton.SetData(new[] { easyColor });
            normalButton = new Texture2D(GameScreenManager.GraphicsDevice, 1, 1);
            normalButton.SetData(new[] { normalColor });

            startDescription = "Start With 1 Player";
        }

        public override void HandleInput()
        {
            base.HandleInput();

            if (InputController.IsBackTriggered()){
                GameScreenManager.AddScreen(new MainMenuScreen());
                RemoveSecondaryInputs();
                ExitScreen();
            }
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
                AudioController.StopSong();
                LoadingScreen.Load(GameScreenManager, true, new GameplayScreen(gameStartDescription));
            }

            if(!buttonCooldown)
            {
                int numModes = Enum.GetNames(typeof(Mode)).Length;
                if (joinedPlayers[0].LeftTriggered() && !InputController.KeyCooldowns.ContainsKey(joinedPlayers[0].LeftKey))
                {
                    buttonPressed = true;
                    selectedMode = (selectedMode == 0) ? (Mode)(numModes - 1) : (selectedMode - 1);
                }
                if (joinedPlayers[0].RightTriggered() && !InputController.KeyCooldowns.ContainsKey(joinedPlayers[0].RightKey))
                {
                    buttonPressed = true;
                    selectedMode = (selectedMode == (Mode)(numModes - 1)) ? 0 : (selectedMode + 1);
                }
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            startCoolDown -= gameTime.ElapsedGameTime.Milliseconds;
            if (buttonPressed)
            {
                buttonPressed = false;
                buttonCooldown = true;
                lastPressed = gameTime.TotalGameTime.TotalMilliseconds;
            }
            if (buttonCooldown) // enough time elapsed since last pressed
            {
                double diff = gameTime.TotalGameTime.TotalMilliseconds - lastPressed;
                if ((int)diff > 250)
                {
                    buttonCooldown = false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

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

            Vector2 textLength = descriptionFont.MeasureString(playerNumber) * playerNumberScale;
            int topMargin = (int) (marginY * playerFrameHeight);
            Vector2 textPosition = new Vector2(playerFrame.X + (playerFrame.Width - textLength.X) / 2, playerFrame.Y + topMargin);
            spriteBatch.DrawString(descriptionFont, playerNumber, textPosition, Color.White, 0f, Vector2.Zero, playerNumberScale, SpriteEffects.None, 0f);

            int spriteWidth = (int)(playerSprite.Width * spriteScale);
            int spriteHeight = (int)(playerSprite.Height * spriteScale);
            int playerControlWidth = (int)(playerControl.Width * playerControlScale);
            int playerControlHeight = (int)(playerControl.Height * playerControlScale);
            int offSetX = (int)((playerFrame.Width - (spriteWidth + playerControlWidth)) / 3);
            int offsetY = (int)((playerFrame.Height - (textLength.Y + 2 * topMargin + spriteHeight)) / 2 + textLength.Y + topMargin);
            Vector2 spritePosition = new Vector2(playerFrame.X + offSetX, playerFrame.Y + offsetY);

            offsetY = (int)((playerFrame.Height - (textLength.Y + 2 * topMargin + playerControlHeight)) / 2 + textLength.Y + topMargin);
            Vector2 playerControlPosition =  new Vector2(spritePosition.X + offSetX + spriteWidth, playerFrame.Y + offsetY);

            spriteBatch.Draw(playerSprite, spritePosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(playerControl, playerControlPosition, null, Color.White, 0f, Vector2.Zero, playerControlScale, SpriteEffects.None, 0f);

            // Draw the start button only for player 1
            if(playerNumber.Equals("Player 1"))
            {
                DrawStartButton(playerFrame);
            }
        }

        private void DrawStartButton(Rectangle playerFrame)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            int offsetX, offsetY;
            int startBarHeight = (int)(startBarHeightScale * playerFrame.Height);
            int buttonWidth = (int)(modeButtonWidth * playerFrame.Width);
            int buttonHeight = (int)(modeButtonHeight * startBarHeight);

            // Start Bar
            offsetY = (int)(playerFrame.Y + (1.0f - startBarHeightScale) * playerFrame.Height);
            Rectangle startBGPosition = new Rectangle(playerFrame.X, offsetY, playerFrame.Width, startBarHeight);
            spriteBatch.Draw(startBackground, startBGPosition, Color.White);

            // Start description with number of players
            Vector2 textLength = descriptionFont.MeasureString(startDescription) * startTextScale;
            Vector2 textPosition = new Vector2(playerFrame.X + (playerFrame.Width - textLength.X) / 2, offsetY);
            spriteBatch.DrawString(descriptionFont, startDescription, textPosition, Color.White, 0f, Vector2.Zero, startTextScale, SpriteEffects.None, 0f);

            // Easy mode button
            offsetX = (int)((1 - 2 * modeButtonWidth - modeButtonGap) * playerFrame.Width / 2);
            offsetY = (int)((startBarHeight - buttonHeight - textLength.Y) / 2 + textLength.Y);
            Rectangle easyButtonPosition = new Rectangle(startBGPosition.X + offsetX, startBGPosition.Y + offsetY, buttonWidth, buttonHeight);
            spriteBatch.Draw(easyButton, easyButtonPosition, Color.White);

            Vector2 modetextLength = descriptionFont.MeasureString(modeEasy) * modeTextScale;
            textPosition = new Vector2(easyButtonPosition.X, easyButtonPosition.Y) +  new Vector2((easyButtonPosition.Width - modetextLength.X) / 2, (easyButtonPosition.Height - modetextLength.Y) / 2);
            spriteBatch.DrawString(descriptionFont, modeEasy, textPosition, Color.White, 0f, Vector2.Zero, modeTextScale, SpriteEffects.None, 0f);

            // Normal mode button
            offsetX = (int)((1 - 2 * modeButtonWidth - modeButtonGap) * playerFrame.Width / 2 + (modeButtonGap + modeButtonWidth) * playerFrame.Width);
            offsetY = (int)((startBarHeight - buttonHeight - textLength.Y) / 2 + textLength.Y);
            Rectangle normalButtonPosition = new Rectangle(startBGPosition.X + offsetX, startBGPosition.Y + offsetY, buttonWidth, buttonHeight);
            spriteBatch.Draw(normalButton, normalButtonPosition, Color.White);

            modetextLength = descriptionFont.MeasureString(modeNormal) * modeTextScale;
            textPosition = new Vector2(normalButtonPosition.X, normalButtonPosition.Y) + new Vector2((normalButtonPosition.Width - modetextLength.X) / 2, (normalButtonPosition.Height - modetextLength.Y) / 2);
            spriteBatch.DrawString(descriptionFont, modeNormal, textPosition, Color.White, 0f, Vector2.Zero, modeTextScale, SpriteEffects.None, 0f);

            // Draw indicator for selected mode
            switch (selectedMode)
            {
                case Mode.Easy: DrawSelectedMode(easyButtonPosition); break;

                case Mode.Normal: DrawSelectedMode(normalButtonPosition); break;

                default: break;
            }
            
        }

        private void DrawSelectedMode(Rectangle modeButton)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            // Start button
            int rightMargin = (int)(modeButton.Width * marginStartButton);
            int offsetX = (int)(modeButton.Width - (rightMargin + startButton.Width * startButtonScale));
            int offsetY = (int)((modeButton.Height - startButton.Height * startButtonScale) / 2);
            Vector2 startButtonPosition = new Vector2(modeButton.X + offsetX, modeButton.Y + offsetY);
            spriteBatch.Draw(startButton, startButtonPosition, null, Color.White, 0f, Vector2.Zero, startButtonScale, SpriteEffects.None, 1f);

            //Frame around selected button
            int frameThickness = 2;
            Rectangle frame = new Rectangle(modeButton.X - frameThickness, modeButton.Y - frameThickness, modeButton.Width + 2 * frameThickness, modeButton.Height + 2 * frameThickness);

            Texture2D frameTexture = new Texture2D(GameScreenManager.GraphicsDevice,frame.Width,frame.Height);
            Color[] framePixels = new Color[frame.Width * frame.Height];
            for (int i = 0; i < frame.Width; i++)
            {
                for (int j = 0; j < frame.Height; j++)
                {
                    if (i <= frameThickness || i >= frame.Width - frameThickness -1 || j <= frameThickness || j >= frame.Height - frameThickness -1)
                    {
                        framePixels[j * frame.Width + i] = Color.White;
                    }
                    else
                    {
                        framePixels[j * frame.Width + i] = Color.Transparent;
                    }
                    
                }
            }
            frameTexture.SetData(framePixels);

            spriteBatch.Draw(frameTexture, frame, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
