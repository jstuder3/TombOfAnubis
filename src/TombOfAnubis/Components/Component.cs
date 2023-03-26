using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Component
    {
        public bool Active { get; set; } = true;
        public Entity Entity { get; set; }

        public virtual void DeleteComponent() {
            Active = false;
        } //removal of component (usually only entails deregisterring from the corresponding system, if there is one)
    }

 

}
