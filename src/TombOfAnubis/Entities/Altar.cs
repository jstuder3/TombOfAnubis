using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TombOfAnubis
{
    public class Altar : Entity
    {
        public Altar(Vector2 position, Vector2 scale, Texture2D texture, int numPlayers)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, 1);
            AddComponent(sprite);

            Inventory inventory = new Inventory(numPlayers, 0, this);
            AddComponent(inventory);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);
        }
    }
}
