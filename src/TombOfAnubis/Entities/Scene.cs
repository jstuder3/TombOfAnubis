using Microsoft.Xna.Framework;

namespace TombOfAnubis
{
    public class Scene : Entity
    {
        public Scene(Vector2 worldOrigin)
        {
            Transform transform = new Transform(worldOrigin);
            AddComponent(transform);
        }
    }
}
