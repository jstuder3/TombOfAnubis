using Microsoft.Xna.Framework;

namespace TombOfAnubis
{
    public class Scene : Entity
    {
        /// <summary>
        /// The scene entity is the root entity of the session. The transform describes the origin and scale of the scene and is changed for every player.
        /// </summary>
        /// <param name="worldOrigin"></param>
        public Scene(Vector2 worldOrigin, Vector2 scale, Vector2 minimapScale)
        {
            Transform transform = new Transform(worldOrigin, scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(worldOrigin, minimapScale, Visibility.Minimap);
            AddComponent(minimapTransform);
        }
    }
}
