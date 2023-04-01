using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class DiscoverySystem : BaseSystem<Discovery>
    {
        public Scene Scene { get; set; }
        private Map map;
        public DiscoverySystem(Scene scene) 
        { 
            Scene = scene;
        }

        public override void Update(GameTime deltaTime)
        {
            if(map == null) { return; }
            List<Character> characters = Scene.GetChildrenOfType<Character>();
            foreach (Character character in characters)
            {
                Transform characterTransform = character.GetComponent<Transform>();
                Point currentTileCoordinates = map.PositionToTileCoordinate(characterTransform.Position);

                Point downright = currentTileCoordinates + new Point(1, 1);
                Point right = currentTileCoordinates + new Point(1, 0);
                Point upright = currentTileCoordinates + new Point(1, -1);
                Point up = currentTileCoordinates + new Point(0, -1);
                Point upleft = currentTileCoordinates + new Point(-1, -1);
                Point left = currentTileCoordinates + new Point(-1, 0);
                Point downleft = currentTileCoordinates + new Point(-1, 1);
                Point down = currentTileCoordinates + new Point(0, 1);

                List<Point> neightbourTiles = new List<Point>() { currentTileCoordinates, downright, right, upright, up, upleft, left, downleft, down };

                foreach(Point neighbour in neightbourTiles)
                {
                    if (map.ValidTileCoordinates(neighbour))
                    {
                        Session.GetInstance().MapTiles[neighbour.X, neighbour.Y].GetComponent<Discovery>().Discovered = true;
                    }
                }
            }
        }

        public void SetMap(Map map) { 
            this.map = map;
        }
    }
}
