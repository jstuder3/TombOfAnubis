#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sdcb.FFmpeg.Raw;
using System;
using TombOfAnubisContentData;
#endregion

namespace TombOfAnubis
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    /// <remarks>
    /// Similar to a class found in the Game State Management sample on the 
    /// XNA Creators Club Online website (http://creators.xna.com).
    /// </remarks>
    class MenuEntry
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;


        /// <summary>
        /// The font used for this menu item.
        /// </summary>
        SpriteFont spriteFont;


        /// <summary>
        /// The position of this menu item on the screen.
        /// </summary>
        Vector2 position;


        /// <summary>
        /// A description of the function of the button.
        /// </summary>
        private string description;


        /// <summary>
        /// An optional texture drawn with the text.
        /// </summary>
        /// <remarks>If present, the text will be centered on the texture.</remarks>
        private Texture2D texture;
        private float textureScale = 1.0f;


        /// <summary>
        /// Animation of texture when entry is selected
        /// </summary>
        private Animation textureAnimation;

        /// <summary>
        /// Stores whether this entry was the most recently selected MenuEntry
        /// </summary>
        private bool prevSelected = false;
        private GameTime prevSelectedTime;

        #endregion


        #region Properties


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        /// <summary>
        /// Gets or sets the font used to draw this menu entry.
        /// </summary>
        public SpriteFont Font
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }


        /// <summary>
        /// Gets or sets the position of this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }


        /// <summary>
        /// A description of the function of the button.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        /// <summary>
        /// An optional texture drawn with the text.
        /// </summary>
        /// <remarks>If present, the text will be centered on the texture.</remarks>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public float TextureScale
        {
            get { return textureScale; }
            set { textureScale = value; }
        }


        /// <summary>
        /// Animation of texture when entry is selected
        /// </summary>
        public Animation TextureAnimation
        {
            get { return textureAnimation; }
            set { textureAnimation = value; }
        }

        /// <summary>
        /// Stores whether this entry was the most recently selected MenuEntry
        /// </summary>
        public bool PrevSelected
        {
            get { return prevSelected; }
            set { prevSelected = value; }
        }

        public GameTime PrevSelectedTime
        {
            get { return prevSelectedTime; }
            set { prevSelectedTime = value; }
        }

        #endregion


        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text)
        {
            this.text = text;
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        { 
            if (isSelected && prevSelected)
            {
                double timeElapsedPrevSelect = gameTime.TotalGameTime.Subtract(prevSelectedTime.TotalGameTime).TotalMilliseconds;
                double animationDuration = textureAnimation.AnimationClips[1].GetTotalClipDuration();
                if (timeElapsedPrevSelect >= animationDuration)
                {
                    textureAnimation.SetActiveClip(AnimationClipType.ActiveEntry);
                } 
            }
            else if (!isSelected && prevSelected)
            {
                textureAnimation.SetActiveClip(AnimationClipType.InactiveEntry);
                prevSelected = false;
            }
            else if (isSelected && !prevSelected)
            {
                textureAnimation.SetActiveClip(AnimationClipType.TransitionEntry);
                prevSelectedTime = gameTime;
                prevSelected = true;
            }
            else
            {
                textureAnimation.SetActiveClip(AnimationClipType.InactiveEntry);
            }
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Fonts.MenuSelectedColor : Fonts.TitleColor;

            // Draw text, centered on the middle of each line.
            GameScreenManager screenManager = screen.GameScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            if (texture != null)
            {
                //spriteBatch.Draw(texture, position, Color.White);
                spriteBatch.Draw(texture, position, textureAnimation.DefaultSourceRectangle, Color.White, 0f, Vector2.Zero, textureScale, SpriteEffects.None, 0f);
                if ((spriteFont != null) && !String.IsNullOrEmpty(text))
                {
                    Vector2 textSize = spriteFont.MeasureString(text);
                    Vector2 textPosition = position + new Vector2(
                        (float)Math.Floor((texture.Width * textureScale - textSize.X) / 2),
                        (float)Math.Floor((texture.Height * textureScale - textSize.Y) / 2));
                    spriteBatch.DrawString(spriteFont, text, textPosition, color);
                }
            }
            else if ((spriteFont != null) && !String.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(spriteFont, text, position, color);
            }
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return Font.LineSpacing;
        }


        #endregion
    }
}
