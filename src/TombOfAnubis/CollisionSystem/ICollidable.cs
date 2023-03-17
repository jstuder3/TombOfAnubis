using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public interface ICollidable
    { 
        public Collider Collider { get; set; }
        public abstract void HandleCollision(ICollidable other);

    }
}
