using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public enum ColliderType
    {
        Rectangle,
        Box,
        Circle,
        Sphere,
        Capsule
    }

    public class Collider : Component
    {
        public ColliderType colliderType;
        public Collider() { CollisionSystem.Register(this); }

        virtual public bool Intersects(Collider other)
        {
            throw new NotImplementedException();
        }
    }
}
