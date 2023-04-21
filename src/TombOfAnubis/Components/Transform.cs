using Microsoft.Xna.Framework;

namespace TombOfAnubis
{
    public class Transform : Component
    {
        private Vector2 position;
        /// <summary>
        /// Position of the top left corner of the entity relative to its parent
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                if (Entity != null && Entity.Parent != null)
                {
                    foreach (Transform transform in Entity.GetComponentsOfType<Transform>())
                    {
                        if (!transform.Equals(this) && transform.Visibility == Visibility.Minimap)
                        {
                            Vector2 entitySize = Entity.Size();
                            Vector2 entityCenter = Position + entitySize / 2f;

                            Vector2 minimapEntitySize = Entity.MinimapSize();
                            Vector2 minimapEntityCenter = Position + minimapEntitySize / 2f;

                            transform.Position = Position + entityCenter - minimapEntityCenter;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Scale of this entity relative to its parent
        /// </summary>
        public Vector2 Scale { get; set; }

        public Transform(Vector2 position, Vector2 scale, Visibility visibility)
        {
            Visibility = visibility;
            Position = position;
            Scale = scale;
        }

        /// <summary>
        /// Transforms the position and scale from relative to parent coordinates to world coordinates
        /// </summary>
        public Transform ToWorld()
        {
            if (Entity.Parent != null && Entity.Parent.GetComponent<Transform>() != null)
            {
                Transform parentWorld = Entity.Parent.GetComponent<Transform>().ToWorld();

                Transform worldTransform = new Transform(Position * parentWorld.Scale + parentWorld.Position, Scale * parentWorld.Scale, Visibility);
                worldTransform.Entity = Entity;
                return worldTransform;
                
            }
            else
            {
                return this;
            }
        }

    }
}
