using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class RectangleCollider : Collider
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public RectangleCollider(Vector2 position, Vector2 size)
        {
            Size = size;
            Position = position;
        }
    }
}
