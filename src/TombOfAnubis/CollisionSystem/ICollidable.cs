using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.CollisionSystem
{
    internal interface ICollidable
    {
        Collider collider { get; set; }
        public void handleCollision(ICollidable other);

    }
}
