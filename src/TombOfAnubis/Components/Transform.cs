using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        /// <summary>
        /// Scale of this entity relative to its parent
        /// </summary>
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
        /// Transforms the position and scale from relative to parent coordinates to world coordinates
        /// </summary>
        public Transform ToWorld()
        {
            if(Entity.Parent != null && Entity.Parent.GetComponent<Transform>() != null)
            {
                Transform parentWorld = Entity.Parent.GetComponent<Transform>().ToWorld();
                return new Transform(Position + parentWorld.Position, Scale * parentWorld.Scale);
            }
            else
            {
                return this;
            }
        }
    }
}
