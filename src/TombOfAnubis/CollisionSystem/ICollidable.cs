using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    internal interface ICollidable
    {
        Collider collider { get; set; }
        public void HandleCollision(ICollidable other);

    }
}
