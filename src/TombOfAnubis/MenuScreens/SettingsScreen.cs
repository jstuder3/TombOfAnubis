using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis.ScreenManager;

namespace TombOfAnubis.MenuScreens
{
    class SettingsScreen : GameScreen
    {
        #region Fields

        private Texture2D backgroundTexture;
        private Rectangle backgroundPosition;

        private SpriteFont settingFont;
        private SpriteFont titleFont;

        private Texture2D settingsScroll;
        private int settingsScrollWidth = 800, settingsScrollHeight = 400;
        private Vector2 settingsScrollScale = new Vector2(1.2f, 3f);
        private Vector2 settingsScrollPosition;

        private string title = "Settings";
        private Vector2 titlePosition;

        private float marginX = 0.22f, marginY = 0.15f;
        private float spacingX = 0.5f, spacingY = 0.05f;

        List<SettingsEntry> settingsEntries;
        protected int settingsSelectedEntry = 0;

        SettingsEntry resolutionSetting, fullscreenSetting, bgMusicVolumeSetting, soundFXVolumeSetting, saveButton;

        Settings settings;

        Texture2D arrowTexture;
        float selectBoxWidth = 0.28f;
        float arrowScale = 0.1f;

        float sliderButtonWidth = 0.01f, sliderButtonHeight = 0.025f;
        float sliderLength = 0.28f, sliderThickness = 0.005f;
        float sliderIncrement = 0.025f;

        // Relative to Tickbox size
        float tickBoxActiveSize = 0.7f;

        #endregion

        #region Properties

        protected List<SettingsEntry> SettingsEntries
        {
            get { return settingsEntries; }
            set { settingsEntries = value; }
        }

        protected bool buttonPressed;
        protected bool buttonCooldown;
        protected TimeSpan lastPressed;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsScreen()
        {
            settings = Settings.Read();
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            buttonCooldown = true;
            buttonPressed = true;
            lastPressed = TimeSpan.Zero;


            /// <summary>
            /// Add entries to the Settings Screen
            /// </summary>


            resolutionSetting = new SettingsEntry("Resolution", SettingsType.SelectBox, ControlType.Resolution);
            fullscreenSetting = new SettingsEntry("Fullscreen", SettingsType.TickBox, ControlType.Fullscreen);
            bgMusicVolumeSetting = new SettingsEntry("Music Volume", SettingsType.Slider, ControlType.Music);
            soundFXVolumeSetting = new SettingsEntry("SoundFX Volume", SettingsType.Slider, ControlType.SoundFX);
            saveButton = new SettingsEntry("Apply", SettingsType.SaveButton, ControlType.Save);

            bgMusicVolumeSetting.SliderStatus = settings.VolumeSetting;
            soundFXVolumeSetting.SliderStatus = settings.SoundFXVolumeSetting;
            fullscreenSetting.TickBoxStatus = settings.IsFullscreen;

            //SettingsEntries = new List<SettingsEntry> { fullscreenSetting, bgMusicVolumeSetting, soundFXVolumeSetting, saveButton };
            SettingsEntries = new List<SettingsEntry> {fullscreenSetting, bgMusicVolumeSetting, soundFXVolumeSetting, saveButton };
        }


        #endregion

        #region Load Content

        public override void LoadContent()
        {

            // Load the textures
            ContentManager content = GameScreenManager.Game.Content;
            backgroundTexture = content.Load<Texture2D>("Textures/Menu/main_menu_bg");
            settingsScroll = content.Load<Texture2D>("Textures/Menu/Scroll");
            
            arrowTexture = content.Load<Texture2D>("Textures/Menu/Indicator");

            settingFont = Fonts.DisneyHeroicFont;
            titleFont = Fonts.SettingsTitleFont;
            foreach (SettingsEntry entry in SettingsEntries)
            {
                entry.EntryFont = settingFont;
                entry.SetColor(Fonts.MenuSelectedColor, Color.Black);
            }

            // Now that they have textures, set the proper positions on the menu entries
            SetElementPosition(new Viewport(0, 0, ResolutionController.RenderTarget.Width, ResolutionController.RenderTarget.Height));

            base.LoadContent();
        }

        public void SetElementPosition(Viewport viewport)
        {
            // Main Screen

            int screenWidth = viewport.Width;
            int screenHeight = viewport.Height;

            backgroundPosition = new Rectangle(0, 0, screenWidth, screenHeight);

            // Due to rotation height and width are flipped
            float textureWidth = ((float)settingsScrollHeight / screenWidth) * settingsScrollScale.Y;
            float textureHeight = ((float)settingsScrollWidth / screenHeight) * settingsScrollScale.X;

            float absMarginX = marginX * textureWidth;
            float absMarginY = marginY * textureHeight;

            float absSpacingX = spacingX * textureWidth;
            float absSpacingY = spacingY * textureWidth;

            float offsetX = (1.0f - textureWidth) / 2;
            float offsetY = (1.0f - textureHeight) / 2;
            settingsScrollPosition = GetRelativePosition(viewport, 0.5f, 0.5f);

            Vector2 titleSize = titleFont.MeasureString(title);
            titleSize = Vector2.Divide(titleSize, new Vector2(screenWidth, screenHeight));
            float titleOffsetX = offsetX + (textureWidth - titleSize.X) / 2;
            float titleOffsetY = offsetY + absMarginY;
            titlePosition = GetRelativePosition(viewport, titleOffsetX, titleOffsetY);

            // The first MenuEntry element is drawn at this relative vertical coordinate
            float entryStart = titleOffsetY + titleSize.Y + absMarginY;
            offsetY = entryStart;

            for (int i = 0; i < SettingsEntries.Count; i++)
            {
                SettingsEntry entry = SettingsEntries[i];
                entry.EntryNamePosition = GetRelativePosition(viewport, offsetX + absMarginX, offsetY);
                entry.ElementPosition = GetRelativePosition(viewport, offsetX + absSpacingX, offsetY);

                float stringHeight = settingFont.MeasureString(SettingsEntries[i].EntryName).Y / screenHeight;
                offsetY += absSpacingY + stringHeight;

                switch (entry.EntryType)
                {
                    case SettingsType.TickBox:
                        {
                            int tickBoxSize = (int)(stringHeight * screenHeight);
                            Rectangle untickedBox = new Rectangle((int)entry.ElementPosition.X, (int)entry.ElementPosition.Y, tickBoxSize, tickBoxSize);
                            int posX = (int)(entry.ElementPosition.X + (1 - tickBoxActiveSize) * tickBoxSize / 2);
                            int posY = (int)(entry.ElementPosition.Y + (1 - tickBoxActiveSize) * tickBoxSize / 2);
                            int activeSize = (int)(tickBoxActiveSize * tickBoxSize);
                            Rectangle tickedBox = new Rectangle(posX, posY, activeSize, activeSize);

                            entry.SetTickBoxParams(untickedBox, tickedBox);
                            break;
                        }

                    case SettingsType.Slider:
                        {
                            float relButtonWidth = sliderButtonWidth * textureWidth;
                            float relButtonHeight = sliderButtonHeight * textureHeight;
                            int absSliderLength = (int)(sliderLength * textureWidth * screenWidth);
                            int absSliderThickness = (int)(sliderThickness * textureHeight * screenHeight);

                            int posX = (int)entry.ElementPosition.X;
                            int posY = (int)(entry.ElementPosition.Y + (stringHeight - relButtonHeight) * screenHeight / 2);
                            int absButtonWidth = (int)(relButtonWidth * screenWidth);
                            int absButtonHeight = (int)(relButtonHeight * screenHeight);  
                            Rectangle sliderButton = new Rectangle(posX, posY, absButtonWidth, absButtonHeight);

                            entry.SetSliderParams(sliderButton, absSliderLength, absSliderThickness, sliderIncrement);
                            break;
                        }

                    case SettingsType.SaveButton:
                        {
                            float stringWidth = entry.EntryFont.MeasureString(entry.EntryName).X / screenWidth;
                            offsetX = (1 - stringWidth) / 2;
                            offsetY = textureHeight - absMarginX - stringHeight;
                            entry.EntryNamePosition = GetRelativePosition(viewport, offsetX, offsetY);
                            break;
                        }

                }
            }

        }

        /// <summary>
        /// Returns the position on viewport relative to its width and height
        /// </summary>
        public Vector2 GetRelativePosition(Viewport viewport, float offsetX, float offsetY)
        {
            int xPos = (int)(viewport.Width * offsetX);
            int yPos = (int)(viewport.Height * offsetY);
            Vector2 relPosition = new Vector2(xPos, yPos);

            return relPosition;
        }

        #endregion

        #region Handle Input

        public override void HandleInput()
        {

            if (!buttonCooldown)
            {
                SettingsEntry entry = settingsEntries[settingsSelectedEntry];

                // Move to the previous settings entry
                if (InputController.IsUpTriggered())
                {
                    AudioController.PlaySoundEffect("menuSelect");
                    settingsSelectedEntry--;
                    if (settingsSelectedEntry < 0)
                        settingsSelectedEntry = settingsEntries.Count - 1;
                    buttonPressed = true;
                }

                // Move to the next settings entry
                if (InputController.IsDownTriggered())
                {
                    AudioController.PlaySoundEffect("menuSelect");
                    settingsSelectedEntry = (settingsSelectedEntry + 1) % settingsEntries.Count;
                    buttonPressed = true;
                }

                // Decrease slider or show left option
                if (InputController.IsleftTriggered())
                {
                    buttonPressed = true;

                    switch (entry.EntryType)
                    {
                        case SettingsType.SelectBox: 
                            {
                                int numOptions = entry.options.Count;
                                // Avoid negative index
                                entry.StatusIdx = ((entry.StatusIdx - 1) % numOptions  + numOptions) % numOptions;
                                entry.SelectBoxStatus = entry.options[entry.StatusIdx];
                                break;
                            }
                        case SettingsType.Slider: 
                            {
                                entry.UpdateSliderStatus(false);

                                switch (entry.ControlType)
                                {
                                    case ControlType.Music: { 
                                            MediaPlayer.Volume = entry.SliderStatus;
                                            settings.VolumeSetting = entry.SliderStatus;

                                            break; }
                                    case ControlType.SoundFX: { 
                                            SoundEffect.MasterVolume = entry.SliderStatus;
                                            settings.SoundFXVolumeSetting = entry.SliderStatus;
                                            break; }
                                    default: { break; }
                                }
                                break;
                            }
                        default: { break; }
                    }
                }

                // Increase slider or show right option
                if (InputController.IsRightTriggered())
                {
                    buttonPressed = true;

                    switch (entry.EntryType)
                    {
                        case SettingsType.SelectBox: 
                            { 
                                entry.StatusIdx = (entry.StatusIdx + 1) % entry.options.Count;
                                entry.SelectBoxStatus = entry.options[entry.StatusIdx];
                                break;
                            }
                        case SettingsType.Slider:
                            {
                                entry.UpdateSliderStatus(true);

                                switch (entry.ControlType)
                                {
                                    case ControlType.Music: { 
                                            MediaPlayer.Volume = entry.SliderStatus;
                                            settings.VolumeSetting = entry.SliderStatus;
                                            break; }
                                    case ControlType.SoundFX: { 
                                            SoundEffect.MasterVolume = entry.SliderStatus;
                                            settings.SoundFXVolumeSetting = entry.SliderStatus;
                                            break; }
                                    default : { break; }
                                }
                                break;
                            }
                        default: { break; }
                    }
                }

                // Tick/Untick box or activate save button
                if (InputController.IsUseTriggered())
                {
                    AudioController.PlaySoundEffect("menuAccept");
                    buttonPressed = true;

                    switch (entry.EntryType)
                    {
                        case SettingsType.TickBox: 
                            {
                                entry.TickBoxStatus = !entry.TickBoxStatus;
                                settings.IsFullscreen = entry.TickBoxStatus;
                                ResolutionController.ToggleFullscreen();

                                break;
                            }
                        case SettingsType.SaveButton: 
                            {
                                settings.Write();
                                GameScreenManager.RemoveScreen(this);
                                break;
                            }
                        default: { break; }
                    }
                }
            }
            
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (buttonPressed)
            {
                buttonPressed = false;
                buttonCooldown = true;
                lastPressed = gameTime.TotalGameTime;
            }
            if (buttonCooldown) // enough time elapsed since last pressed
            {
                TimeSpan diff = gameTime.TotalGameTime - lastPressed;
                if (diff.TotalMilliseconds > 250)
                {
                    buttonCooldown = false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            spriteBatch.Begin();

            Rectangle sourceRectangle = new Rectangle(0, 0, 800, 400);
            Vector2 rotateOrigin = new Vector2(settingsScrollWidth / 2, settingsScrollHeight / 2);
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);
            settingsScrollPosition = new Vector2(960, 540);
            spriteBatch.Draw(settingsScroll, settingsScrollPosition, sourceRectangle, Color.White, MathHelper.ToRadians(90), rotateOrigin, settingsScrollScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(titleFont, title, titlePosition, Color.White);

            // Draw each settings entry in turn.
            for (int i = 0; i < settingsEntries.Count; i++)
            {
                SettingsEntry settingsEntry = settingsEntries[i];

                bool isSelected = IsActive && (i == settingsSelectedEntry);

                settingsEntry.Draw(this, isSelected);
            }

            spriteBatch.End();
        }

        #endregion

    }
}
