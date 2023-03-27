using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Anubis : Entity
    {
        public Anubis(Vector2 position, Vector2 scale, Texture2D texture, int maxMovementSpeed, Map map)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, 2);
            AddComponent(sprite);

            Movement movement = new Movement(maxMovementSpeed);
            AddComponent(movement);

            AI ai = new AI(map);
            AddComponent(ai);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);
        }
    }
}
