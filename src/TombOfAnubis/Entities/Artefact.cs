using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Artefact : Entity
    {
        public Artefact(int playerID, Vector2 position, Vector2 scale, Texture2D texture)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, 1);
            AddComponent(sprite);

            Player player = new Player(playerID);
            AddComponent(player);

            // TODO: Add collider
            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);
        }

    }
}
