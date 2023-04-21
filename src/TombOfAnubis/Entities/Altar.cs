using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TombOfAnubis
{
    public class Altar : Entity
    {
        public Altar(Vector2 position, Vector2 scale, Texture2D texture, int numPlayers)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(position, 2f * scale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Sprite sprite = new Sprite(texture, 1, Visibility.Both);
            AddComponent(sprite);

            Inventory inventory = new Inventory(numPlayers, 0, this);
            AddComponent(inventory);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size());
            AddComponent(collider);

            Discovery discovery = new Discovery();
            AddComponent(discovery);
        }
    }
}
