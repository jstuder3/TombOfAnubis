using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class BoxCollider : Collider
    {
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        public BoxCollider(Vector3 position, Vector3 size) : base() {
            Position = position;
            Size = size;
        }
    }
}
