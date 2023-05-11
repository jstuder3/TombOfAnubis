using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TombOfAnubis.MenuScreens;

namespace TombOfAnubis.ScreenManager
{
    public enum SettingsType
    {
        SelectBox,
        TickBox,
        Slider,
        SaveButton,
    }

    public enum ControlType
    {
        Resolution,
        Fullscreen,
        Music,
        SoundFX,
        Save,
    }

    class SettingsEntry
    {
        #region Fields

        string entryName;
        SpriteFont entryFont;
        Vector2 entryNamePosition;
        // Location of entryElement of SettingsType
        Vector2 elementPosition;

        SettingsType entryType;
        ControlType controlType;

        public Color activeColor, inactiveColor;

        public Rectangle sliderButtonPosition;
        public int sliderLength, sliderThickness;
        public float sliderIncrement;
        float sliderStatus;

        public Rectangle selectBox;
        public Texture2D boxArrow;
        public float boxArrowScale;
        public List<string> options;
        int statusIdx;
        string selectBoxStatus;

        public Rectangle tickedBox, untickedBox;
        bool tickBoxStatus;

        public Texture2D saveButton;
        public float saveButtonScale;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the name of this settings entry.
        /// </summary>
        public string EntryName
        {
            get { return entryName; }
            set { entryName = value; }
        }


        /// <summary>
        /// Gets or sets the font used to draw this settings entry.
        /// </summary>
        public SpriteFont EntryFont
        {
            get { return entryFont; }
            set { entryFont = value; }
        }


        /// <summary>
        /// Gets or sets the position of this menu entry.
        /// </summary>
        public Vector2 EntryNamePosition
        {
            get { return entryNamePosition; }
            set { entryNamePosition = value; }
        }

        /// <summary>
        /// Gets or sets the position of interactive element of the settings entry.
        /// </summary>
        public Vector2 ElementPosition
        {
            get { return elementPosition; }
            set { elementPosition = value; }
        }

        public SettingsType EntryType
        {
            get { return entryType; }
            set { entryType = value; }
        }

        public ControlType ControlType
        {
            get { return controlType; }
            set { controlType = value; }
        }

        public float SliderStatus
        {
            get { return sliderStatus; }
            set { sliderStatus = value; }
        }

        public string SelectBoxStatus
        {
            get { return selectBoxStatus; }
            set { selectBoxStatus = value; }
        }

        public int StatusIdx
        {
            get { return statusIdx; }
            set { statusIdx = value; }
        }

        public bool TickBoxStatus
        {
            get { return tickBoxStatus; }
            set { tickBoxStatus = value; }
        }
        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public SettingsEntry(string name, SettingsType settingsType, ControlType controlType)
        {
            this.entryName = name;
            this.entryType = settingsType;
            this.controlType = controlType;
        }

        public void SetSelectBoxParams(Rectangle box, Texture2D arrow, float scale, List<string> options)
        {
            this.selectBox = box;
            this.boxArrow = arrow;
            this.boxArrowScale = scale;
            this.options = options;
            this.StatusIdx = 0;
            this.SelectBoxStatus = options[statusIdx];
        }

        public void SetTickBoxParams(Rectangle untickedBox, Rectangle tickedBox)
        {
            this.untickedBox = untickedBox;
            this.tickedBox = tickedBox;

        }

        public void SetColor(Color active, Color inactive)
        {
            this.activeColor = active;
            this.inactiveColor = inactive;
        }

        public void SetSliderParams(Rectangle buttonPosition, int length, int thickness, float increment)
        {
            this.sliderButtonPosition = buttonPosition;
            this.sliderLength = length;
            this.sliderThickness = thickness;
            this.sliderIncrement = increment;
        }

        public void UpdateSliderStatus(bool increase)
        {
            float newValue = increase ? this.sliderStatus + this.sliderIncrement : this.sliderStatus - this.sliderIncrement;
            newValue = (newValue < 0) ? 0 : newValue;
            newValue = (newValue > 1) ? 1 : newValue;
            this.sliderStatus = newValue;
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(SettingsScreen screen, bool isSelected)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? activeColor : inactiveColor;

            // Draw text, centered on the middle of each line.
            GameScreenManager screenManager = screen.GameScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            spriteBatch.DrawString(entryFont, entryName, entryNamePosition, color);

            switch (entryType)
            {
                case SettingsType.SelectBox: DrawSelectBox(screen); break;

                case SettingsType.TickBox: DrawTickBox(screen);  break;

                case SettingsType.Slider: DrawSlider(screen);  break;

                case SettingsType.SaveButton: break;

                default: break;
            }

        }

        public void DrawSelectBox(SettingsScreen screen)
        {
            GameScreenManager screenManager = screen.GameScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            Texture2D boxBackground = new Texture2D(screenManager.GraphicsDevice, 1, 1);
            boxBackground.SetData(new[] { Color.Black });
            float transparency = 0.7f;

            spriteBatch.Draw(boxBackground, selectBox, Color.White * transparency);

            int arrowPositionX = (int)(selectBox.X + boxArrow.Width * boxArrowScale);
            int arrowPositionY = (int)(selectBox.Y + selectBox.Height / 2);
            Vector2 arrowPosition = new Vector2(arrowPositionX, arrowPositionY);
            Vector2 rotationOrigin = new Vector2(boxArrow.Width / 2, boxArrow.Height / 2);
            spriteBatch.Draw(boxArrow, arrowPosition, null, Color.White, MathHelper.ToRadians(180), rotationOrigin, boxArrowScale, SpriteEffects.None, 0f);

            arrowPositionX = (int)(selectBox.X + selectBox.Width - boxArrow.Width * boxArrowScale);
            arrowPosition = new Vector2(arrowPositionX, arrowPositionY);
            spriteBatch.Draw(boxArrow, arrowPosition, null, Color.White, 0f, rotationOrigin, boxArrowScale, SpriteEffects.None, 0f);


            Vector2 textDimension = entryFont.MeasureString(selectBoxStatus);
            Vector2 textPosition = new Vector2(selectBox.X, selectBox.Y) + new Vector2((selectBox.Width - textDimension.X) / 2, 0);
            spriteBatch.DrawString(entryFont, selectBoxStatus, textPosition, Color.White);
        }

        public void DrawSlider(SettingsScreen screen)
        {
            GameScreenManager screenManager = screen.GameScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            Texture2D sliderActive = new Texture2D(screenManager.GraphicsDevice, 1, 1);
            sliderActive.SetData(new[] { activeColor });

            Texture2D sliderInactive = new Texture2D(screenManager.GraphicsDevice, 1, 1);
            sliderInactive.SetData(new[] { inactiveColor });

            int entryHeight = (int)entryFont.MeasureString(entryName).Y;
            int posX = (int)elementPosition.X;
            int posY = (int)elementPosition.Y + (entryHeight - sliderThickness) / 2;
            int activeBarLength = (int)sliderButtonPosition.X - posX;
            Rectangle sliderLeftBar = new Rectangle(posX, posY, activeBarLength, sliderThickness);
            spriteBatch.Draw(sliderActive, sliderLeftBar, Color.White);

            posX = sliderLeftBar.X + sliderLeftBar.Width;
            int inactiveBarLength = sliderLength - activeBarLength;
            Rectangle sliderRightBar = new Rectangle(posX, posY, inactiveBarLength, sliderThickness);
            spriteBatch.Draw(sliderInactive, sliderRightBar, Color.White);

            posX = (int)(elementPosition.X + sliderStatus * sliderLength);
            sliderButtonPosition.X = posX;
            spriteBatch.Draw(sliderActive, sliderButtonPosition, Color.White);
        }

        public void DrawTickBox(SettingsScreen screen)
        {
            GameScreenManager screenManager = screen.GameScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            Texture2D unticked = new Texture2D(screenManager.GraphicsDevice, 1, 1);
            unticked.SetData(new[] { inactiveColor });
            float transparency = 0.7f;
            spriteBatch.Draw(unticked, untickedBox, Color.White * transparency);

            if (tickBoxStatus)
            {
                Texture2D ticked = new Texture2D(screenManager.GraphicsDevice, 1, 1);
                ticked.SetData(new[] { activeColor });
                spriteBatch.Draw(ticked, tickedBox, Color.White);
            }
            
        }

        #endregion

    }
}