using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Character : Entity
    {
        public Character(int playerID, Vector2 position, Vector2 scale, Texture2D texture, int maxMovementSpeed)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, 2);
            AddComponent(sprite);

            Player player = new Player(playerID);
            AddComponent(player);

            Input input = new Input();
            AddComponent(input);

            Movement movement = new Movement(maxMovementSpeed);
            AddComponent(movement);

            Inventory inventory = new Inventory(1, 3, this);
            AddComponent(inventory);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);

        }

    }
}
