using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Artefact : Entity
    {


        public Artefact(int playerID, Vector2 position, Vector2 scale, Texture2D texture, bool collidable)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Player player = new Player(playerID);
            AddComponent(player);

            Sprite sprite = new Sprite(texture, 2);
            AddComponent(sprite);
            if (collidable)
            {
                RectangleCollider collider = new RectangleCollider(position, Size());
                AddComponent(collider);
            }
        }

    }
}
