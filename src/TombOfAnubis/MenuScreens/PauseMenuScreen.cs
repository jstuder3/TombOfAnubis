using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Framework.Media;
using MonoGame.Extended.VideoPlayback;

namespace TombOfAnubis
{
    class PauseMenuScreen : MenuScreen
    {
        private Texture2D backgroundTexture;
        private Vector2 backgroundPosition;

        private Texture2D descriptionAreaTexture;
        private Vector2 descriptionAreaPosition;
        private Vector2 descriptionAreaTextPosition;

        private Texture2D iconTexture;
        private Vector2 iconPosition;

        private Texture2D backTexture;
        private Vector2 backPosition;

        private Texture2D selectTexture;
        private Vector2 selectPosition;

        private Texture2D plankTexture1, plankTexture2, plankTexture3;

        MenuEntry resumeGameMenuEntry, exitGameMenuEntry;

        public PauseMenuScreen() : base()
        {
            // add the New Game entry
            resumeGameMenuEntry = new MenuEntry("Resume");
            resumeGameMenuEntry.Description = "Resume the game";
            resumeGameMenuEntry.Font = Fonts.DisneyHeroicFont;
            resumeGameMenuEntry.Position = new Vector2(715, 0f);
            resumeGameMenuEntry.Selected += ResumeMenuEntrySelected;
            MenuEntries.Add(resumeGameMenuEntry);

            // create the Exit menu entry
            exitGameMenuEntry = new MenuEntry("Quit");
            exitGameMenuEntry.Description = "Quit the game";
            exitGameMenuEntry.Font = Fonts.DisneyHeroicFont;
            exitGameMenuEntry.Position = new Vector2(720, 0f);
            exitGameMenuEntry.Selected += ExitGameMenuEntrySelected;
            MenuEntries.Add(exitGameMenuEntry);
        }
        public override void LoadContent()
        {
            // TODO: Add actual textures
            // load the textures
            ContentManager content = GameScreenManager.Game.Content;
            backgroundTexture = content.Load<Texture2D>("Textures/Menu/plagiarized_bg");
            descriptionAreaTexture =
                content.Load<Texture2D>("Textures/Menu/MenuTile");
            iconTexture = content.Load<Texture2D>("Textures/Menu/MenuTile");
            plankTexture1 =
                content.Load<Texture2D>("Textures/Menu/MenuTile");
            plankTexture2 =
                content.Load<Texture2D>("Textures/Menu/MenuTile");
            plankTexture3 =
                content.Load<Texture2D>("Textures/Menu/MenuTile");
            backTexture = content.Load<Texture2D>("Textures/Menu/MenuTile");
            selectTexture = content.Load<Texture2D>("Textures/Menu/MenuTile");

            // calculate the texture positions
            Viewport viewport = GameScreenManager.GraphicsDevice.Viewport;
            backgroundPosition = new Vector2(
                (viewport.Width - backgroundTexture.Width) / 2,
                (viewport.Height - backgroundTexture.Height) / 2);
            descriptionAreaPosition = backgroundPosition + new Vector2(158, 130);
            descriptionAreaTextPosition = backgroundPosition + new Vector2(158, 350);
            iconPosition = backgroundPosition + new Vector2(170, 80);
            backPosition = backgroundPosition + new Vector2(225, 610);
            selectPosition = backgroundPosition + new Vector2(1120, 610);

            // set the textures on each menu entry
            resumeGameMenuEntry.Texture = plankTexture3;
            exitGameMenuEntry.Texture = plankTexture1;

            // now that they have textures, set the proper positions on the menu entries
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntries[i].Position = new Vector2(
                    MenuEntries[i].Position.X,
                    500f - ((MenuEntries[i].Texture.Height - 10) *
                        (MenuEntries.Count - 1 - i)));
            }
            //AudioController.PlaySong("background_music");
            base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // draw the background images
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);
            spriteBatch.Draw(descriptionAreaTexture, descriptionAreaPosition,
                Color.White);
            spriteBatch.Draw(iconTexture, iconPosition, Color.White);

            // Draw each menu entry in turn.
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntry.Draw(this, isSelected, gameTime);
            }

            // draw the description text for the selected entry
            MenuEntry selectedMenuEntry = SelectedMenuEntry;
            if ((selectedMenuEntry != null) &&
                !String.IsNullOrEmpty(selectedMenuEntry.Description))
            {
                Vector2 textSize =
                    Fonts.DisneyHeroicFont.MeasureString(selectedMenuEntry.Description);
                Vector2 textPosition = descriptionAreaTextPosition + new Vector2(
                    (float)Math.Floor((descriptionAreaTexture.Width - textSize.X) / 2),
                    0f);
                spriteBatch.DrawString(Fonts.DisneyHeroicFont,
                    selectedMenuEntry.Description, textPosition, Fonts.TitleColor);
            }

            spriteBatch.End();
        }
        void ResumeMenuEntrySelected(object sender, EventArgs e)
        {
            ExitScreen();
        }

        void ExitGameMenuEntrySelected(Object sender, EventArgs e)
        {
            GameScreenManager.ExitGame();
        }

    }
}
