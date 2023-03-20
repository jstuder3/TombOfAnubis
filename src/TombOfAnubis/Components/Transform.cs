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
