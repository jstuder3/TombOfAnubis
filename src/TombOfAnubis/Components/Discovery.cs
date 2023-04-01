using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Discovery : Component
    {
        public bool Discovered { get; set; }
        public Discovery()
        {
            Discovered = false;
            DiscoverySystem.Register(this);
        }

        public override void Delete()
        {
            base.Delete();
            DiscoverySystem.Deregister(this);
        }
    }
}
