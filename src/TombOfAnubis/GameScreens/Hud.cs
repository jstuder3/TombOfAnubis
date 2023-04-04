using Microsoft.Xna.Framework;
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
        private SpriteFont artefactStatusFont;
        private Color artefactStatusColor;


        public Hud(GraphicsDevice graphicsDevice)
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
            artefactStatusFont = Fonts.DisneyHeroicFont;
            artefactStatusColor = Fonts.ArtefactStatusColor;
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
            Session.SetFocusOnMapCenter(viewport);

            Vector2 mapSize = session.Map.MapSize * MinimapScale;
            Vector2 minimapPosition = session.MapTiles[0, 0].GetComponent<Transform>().ToWorld().Position;

            // Minimap background
            session.SpriteSystem.SpriteBatch.Draw(minimapBackground, new Rectangle((int)minimapPosition.X - 1, (int)minimapPosition.Y - 1, (int)mapSize.X + 2, (int)mapSize.Y + 2), Color.White * 0.1f);

            Session.Draw(gameTime);




            session.Scene.GetComponent<Transform>().Scale = Vector2.One;
        }

        private void DrawArtefactInventory(GameTime gameTime, Character character, Viewport characterViewport)
        {
            // TODO: Implement

            int playerID = character.GetComponent<Player>().PlayerID;
            int numCollectedArtefacts = character.GetComponent<Inventory>().ArtefactCount();
            int totalArtefacts = character.GetComponent<Inventory>().ArtefactSlots.Count;

            string artefactStatus = "Artefacts: " + numCollectedArtefacts + "/" + totalArtefacts;
            Vector2 textLength = artefactStatusFont.MeasureString(artefactStatus);
            Vector2 displayPositionOffSet = new Vector2((characterViewport.Width - textLength.X )/2, 0f);
            Vector2 displayPosition = new Vector2(characterViewport.X, characterViewport.Y) + displayPositionOffSet;

            // Session.SetFocusOnPlayer(playerID, characterViewport);
            session.SpriteSystem.SpriteBatch.DrawString(artefactStatusFont, artefactStatus, displayPosition, artefactStatusColor);
            // Session.Draw(gameTime);

        }
    }
}
