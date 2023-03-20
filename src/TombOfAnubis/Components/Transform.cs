using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Transform : Component
    {
        /// <summary>
        /// Position of the top left corner of the entity relative to its parent
        /// </summary>
        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; }

        public Transform(Vector2 position)
        {
            Position = position;
            Scale = Vector2.One;
        }
        public Transform(Vector2 position, Vector2 scale)
        {
            Position = position;
            Scale = scale;
        }

        /// <summary>
        /// Transforms the position from relative to parent coordinates to world coordinates
        /// </summary>
        public Transform ToWorld()
        {
            if(Entity.Parent != null && Entity.Parent.GetComponent<Transform>() != null)
            {
                return new Transform(Position + Entity.Parent.GetComponent<Transform>().ToWorld().Position);
            }
            else
            {
                return this;
            }
        }
    }
}
