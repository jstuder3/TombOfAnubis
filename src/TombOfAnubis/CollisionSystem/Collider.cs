using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.CollisionSystem
{
    enum ColliderType
    {
        Rectangle,
        Box,
        Circle,
        Sphere,
        Capsule
    }

    internal class Collider
    {
        public ColliderType colliderType;
        virtual public bool Intersects(Collider other)
        {
            throw new NotImplementedException();
        }
    }
}
