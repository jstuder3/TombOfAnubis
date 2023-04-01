using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Hud
    {
        public Vector2 MinimapScale {  get; set; }
        private Viewport viewport;
        private GraphicsDevice graphics;
        private Session session;
        private List<Character> characters;
        private List<Viewport> characterViewports;


        public Hud(GraphicsDevice graphicsDevice)
        {
            graphics = graphicsDevice;
            viewport = graphics.Viewport;
            session = Session.GetInstance();
            MinimapScale = Vector2.One / 16;
            characters = session.Scene.GetChildrenOfType<Character>();
            characterViewports = SplitScreen.PlayerViewports;
        }

        public void Draw(GameTime gameTime)
        {
            var tmp = graphics.Viewport;

            graphics.Viewport = viewport;

            DrawMinimap(gameTime);

            for(int i = 0; i < characters.Count; i++)
            {
                DrawArtefactInventory(gameTime, characters[i], characterViewports[i]);
            }
            

            graphics.Viewport = tmp;
        }

        private void DrawMinimap(GameTime gameTime)
        {
            // Background
            //Vector2 mapCenter = new Vector2(
            //    session.Map.MapDimensions.X * session.Map.TileSize.X * session.Scene.GetComponent<Transform>().Scale.X / 2,
            //    session.Map.MapDimensions.Y * session.Map.TileSize.Y * session.Scene.GetComponent<Transform>().Scale.Y / 2);

            session.Scene.GetComponent<Transform>().Scale = MinimapScale;
            Session.SetFocusOnMapCenter(viewport);
            Session.Draw(gameTime);
            session.Scene.GetComponent<Transform>().Scale = Vector2.One;
        }

        private void DrawArtefactInventory(GameTime gameTime, Character character, Viewport characterViewport)
        {
            // TODO: Implement
        }
    }
}
