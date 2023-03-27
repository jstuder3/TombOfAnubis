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
        public Entity Entity { get; set; }

        public BaseSystem<Component> System { get; set; }

        /// <summary>
        /// Deletes the component. Override and call System.Deregister(this) on all registered systems.
        /// </summary>
        public virtual void Delete() {
        }
    }

 

}
