using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Inventory inventory = new Inventory();
            AddComponent(inventory);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);

        }
    }
}
