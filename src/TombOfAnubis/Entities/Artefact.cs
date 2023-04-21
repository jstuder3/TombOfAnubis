using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public class Artefact : Entity
    {
        public Artefact(int playerID, Vector2 position, Vector2 scale, Texture2D texture, bool collidable)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(position, 10f * scale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Player player = new Player(playerID);
            AddComponent(player);

            Sprite sprite = new Sprite(texture, 2, Visibility.Both);
            AddComponent(sprite);

            Discovery discovery = new Discovery();
            AddComponent(discovery);

            if (collidable)
            {
                RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size());
                AddComponent(collider);
            }
        }

    }
}
